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


        // 定期的にコールバックされるタイマー
        Timer timer = new Timer();


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

            StringBuilder sb = new StringBuilder(65535);//256とかでええんか？に特に意味はない
            GetWindowText(GetForegroundWindow(), sb, 65535);

            WorkInfoKey key = new WorkInfoKey(p.ProcessName, sb.ToString());

#if false
            WorkInfoData foundData = null;
            foreach ( var data in this.dataList )
            {
                if ( (data.ExecFileName.Equals(key.ExecFileName)) && (data.TitleBarContent.Equals(key.TitleBarContent)) )
                {
                    foundData = data;
                    break;
                }
            }
#endif

            if ( this.dataMap.ContainsKey(key) )
//            if (foundData != null)
            {
                Console.WriteLine("{0} already exists.", p.ProcessName);

                ((WorkInfoData)this.dataMap[key]).incTestCounter();
                //foundData.incTestCounter();
            }
            else
            {
                Console.WriteLine("{0} is first.", p.ProcessName);

                var value = new WorkInfoData(key);

                this.dataMap.Add(key, value);
                //this.dataList.Add(value);
            }


            // ソートされていたらソートの状態も復元する。ソートしているかどうかは、Update前に取得する。
            DataGridViewColumn sortOrderColumn = null;
            SortOrder sortOrder = SortOrder.None;

            foreach (DataGridViewColumn column in this.dataGridView1.Columns)
            {
                if (column.HeaderCell.SortGlyphDirection != SortOrder.None)
                {
                    sortOrderColumn = column;
                    sortOrder = column.HeaderCell.SortGlyphDirection;

                    break;      // TODO:これでいいんだっけ？複数ソートされていた場合は…？
                }
            }


            // Update
            var bindingList = new SortableBindingList<WorkInfoData>();
            foreach ( var data in this.dataMap.Values )
            {
                bindingList.Add(data);
            }

            this.wrapper = new BindingSource()
            {
                DataSource = bindingList  // this.dataMap.Values
            };

            this.dataGridView1.DataSource = this.wrapper;

            //this.wrapper.ResetBindings(false);

            //this.wrapper.DataSource = this.dataMap.Values;
            //this.dataGridView1.DataSource = this.wrapper;
            //this.wrapper.ResetBindings(false);


            // スクロールバーの位置調整用
            try
            {
                this.dataGridView1.FirstDisplayedScrollingRowIndex = this.nScrollPos;
            }
            catch ( Exception )
            {
                // 行が少なくなった場合など、有効範囲外を指すことになってしまったら0にリセットする。
                // この場合DataGridViewのFirstDisplayedScrollingRowIndexには何も格納してはいけない。
                // 触るとそこでまた例外が発生するかもしれないため。
                this.nScrollPos = 0;
            }


            if (sortOrderColumn != null)
            {
                // ソートされている列がある

                // 並び替えを行う
                //this.dataGridView1.Sort( sortOrderColumn,
                //                         (sortOrder == SortOrder.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending);
                //DataTable dt = (DataTable)this.dataGridView1.DataSource;
                //DataView dv = dt.DefaultView;
                //dv.Sort = sortOrderColumn.Name + @" " + ((sortOrder == SortOrder.Ascending) ? "ASC" : "DESC");

                // TODO:わからん。。。同じ名前の列を探すしかないんか？ まぁまずはやってみるか。
                DataGridViewColumn targetColumn = null;
                foreach (DataGridViewColumn column in this.dataGridView1.Columns)
                {
                    if ( column.Name.Equals(sortOrderColumn.Name) )
                    {
                        targetColumn = column;
                        break;
                    }
                }

                if (targetColumn != null)
                {
                    this.dataGridView1.Sort(targetColumn,
                                            (sortOrder == SortOrder.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending);

                    // 並び替えグリフを変更
                    targetColumn.HeaderCell.SortGlyphDirection = sortOrder;
                }
            }
        }


        public WorkTrackerForm()
        {
            InitializeComponent();
        }


        // なぜかBindingSourceを自前で用意しないと反映されん。。。
        private BindingSource wrapper = null;

        // まーまずMapでいくで。Valuesで。
        private Dictionary<WorkInfoKey, WorkInfoData> dataMap = null;
        // private BindingList<WorkInfoData> dataList = null;


        private void WorkTrackerForm_Load(object sender, EventArgs e)
        {
            this.timer = new Timer();
            this.timer.Tick += new EventHandler(tick);
            this.timer.Interval = 1000;
            this.timer.Enabled = true;
            this.timer.Start();

#if false
            var key1 = new WorkInfoKey("abc.exe", "aaa");
            var key2 = new WorkInfoKey("def.exe", "bbb");
            var key3 = new WorkInfoKey("ghi.exe", "ccc");

            dataMap = new Dictionary<WorkInfoKey, WorkInfoData>()
            {
                { key1, new WorkInfoData(key1) },
                { key2, new WorkInfoData(key2) },
                { key3, new WorkInfoData(key3) },
            };
#endif

            this.dataMap = new Dictionary<WorkInfoKey, WorkInfoData>();
//            this.dataList = new BindingList<WorkInfoData>();
            this.wrapper = new BindingSource()
            {
                DataSource = this.dataMap.Values
//                DataSource = this.dataList
            };


            this.dataGridView1.DataSource = this.wrapper;

            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }



        private void btnSave_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter("out.csv", false, Encoding.GetEncoding("Shift_JIS")))
            {
                try
                {
                    foreach ( var kvp in this.dataMap )
                    {
                        sw.WriteLine( "{0},{1},{2}", kvp.Value.Time, kvp.Value.ExecFileName, kvp.Value.TitleBarContent );
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
