using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;


// https://qiita.com/lusf/items/dcce573787e808ccb0ea  DataGridView BindingSource
// https://gist.github.com/yoshikazuendo/8330429  DataGridView BindingSource myclass
// https://garafu.blogspot.com/2016/09/cs-datagridview-customdata.html  (★) DataSourceはここでうまくいった


namespace ActiveWindowChecker
{
    public partial class WorkTrackerForm : Form
    {
        // アクティブウィンドウを取得するWin32 API
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);



        /// <summary>
        /// 定期的にコールバックされるタイマー
        /// </summary>
        Timer timer = new Timer();

        /// <summary>
        /// カウントしているデータ
        /// TODO:線形検索しているので、いずれハッシュ検索できるようにする
        /// </summary>
        SortableBindingList<WorkInfoData> blist = null;

        /// <summary>
        /// DataGridView.DataSource設定用のデータバインドソース
        /// </summary>
        private BindingSource wrapper = null;


        private void WorkTrackerForm_Load(object sender, EventArgs e)
        {
            this.timer = new Timer();
            this.timer.Tick += new EventHandler(tick);
            this.timer.Interval = 1000;
            this.timer.Enabled = true;
            this.timer.Start();

            // BindingSourceのDataSourceに格納
            this.blist = new SortableBindingList<WorkInfoData>();
            this.wrapper = new BindingSource()
            {
                DataSource = this.blist
            };

            // DataGridViewのDataSourceに格納
            this.dataGridView1.DataSource = this.wrapper;

            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }



        /// <summary>
        /// 1000 [ms]毎に情報取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tick(object sender, EventArgs e)
        {
            Console.WriteLine("ticked.");

            // アクティブウィンドウ(フォアグラウンド)のハンドル取得
            IntPtr hWnd = GetForegroundWindow();

            // プロセスID取得
            int id;
            GetWindowThreadProcessId(hWnd, out id);

            // プロセス情報取得
            Process p = Process.GetProcessById(id);
            if ( p == null )
            {
                return;
            }

            StringBuilder sb = new StringBuilder(65535);    //256とかでええんか？に特に意味はない
            GetWindowText(GetForegroundWindow(), sb, 65535);

            WorkInfoKey key = new WorkInfoKey(p.ProcessName, sb.ToString());


            // まずはすでに存在するデータかどうか検索
            WorkInfoData foundData = null;
            foreach ( var data in this.blist )
            {
                if ( key.Equals(data.key) )
                {
                    foundData = data;
                    break;
                }
            }

            if (foundData != null)
            {
                // すでにあるのでデータ更新のみ
                foundData.incTestCounter();
            }
            else
            {
                // 新規データ
                var value = new WorkInfoData(key);
                this.blist.Add(value);

                value.incTestCounter();
            }


            // 以下でもいいが、ポーリングしたくないのでDataにINotifyPropertyChangedを実装する方式にする
            // this.dataGridView1.Refresh();


// ここで、改めてソートした方がいいかも？
// TODO:以下のやり方でいいのかよくわからない。。。

#if false
            // ソート状態を調べる
            DataGridViewColumn sortOrderColumn = null;
            SortOrder          sortOrder       = SortOrder.None;

            foreach (DataGridViewColumn column in this.dataGridView1.Columns)
            {
                if (column.HeaderCell.SortGlyphDirection != SortOrder.None)
                {
                    sortOrderColumn = column;
                    sortOrder = column.HeaderCell.SortGlyphDirection;

                    break;      // とりあえず、単一ソートのみと仮定
                }
            }

            if (sortOrderColumn != null)
            {
                // ソートされている列があるので、並び替えを行う
                this.dataGridView1.Sort( sortOrderColumn, (sortOrder == SortOrder.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending);
            }
#endif
        }


        public WorkTrackerForm()
        {
            InitializeComponent();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter("out.csv", false, Encoding.GetEncoding("Shift_JIS")))
            {
                try
                {
                    foreach (var data in this.blist)
                    {
                        sw.WriteLine("{0},{1},{2}", data.Time, data.ExecFileName, data.TitleBarContent);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("I/Oエラー:" + ex.Message, "Error");
                }
            }
        }


        /// <summary>
        /// スクロールバーの位置記憶用
        /// </summary>
        int nScrollPos = 0;

        /// <summary>
        /// スクロール時にコールバックされる。スクロールバーの位置を記憶する。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                this.nScrollPos = e.NewValue;
            }
        }
    }
}
