using System;


public class WorkInfoKey
{
    // 実行ファイル名(.exe)
    public string ExecFileName { get; set; }

    // タイトルバーの内容
    public string TitleBarContent { get; set; }

    public WorkInfoKey(string ExecFileName, string TitleBarContent)
    {
        this.ExecFileName = ExecFileName;
        this.TitleBarContent = TitleBarContent;
    }

    // オブジェクト自身の評価値
    public override int GetHashCode()
    {
        return this.ExecFileName.GetHashCode() + this.TitleBarContent.GetHashCode();
    }

    // オブジェクトの比較関数
    public override bool Equals(object obj)
    {
        var rhs = obj as WorkInfoKey;
        if (rhs == null)
        {
            return false;
        }

        return ((this.ExecFileName.Equals(rhs.ExecFileName)) && (this.TitleBarContent.Equals(rhs.TitleBarContent)));
    }
}


public class WorkInfoData
{
    // キーに実行ファイル名やタイトルバー情報があるので、データとして持っておく
    private WorkInfoKey key = null;

    // カウンタ。仮に一秒に一回コールされる関数から呼ばれて+1するので1秒に…ならなさそうだがやってみる。
    private int nTestCounter = 0;
    public int getTestCounter() { return this.nTestCounter; }
    public void setTestCounter(int n) { this.nTestCounter = n; }
    public void incTestCounter() { ++nTestCounter; }


    // 以下DataGridViewのDataSourceとして表示させるデータ。stringじゃないとダメだったと思うので注意。
    public string ExecFileName { get { return key.ExecFileName; } }
    public string Time { get { return nTestCounter.ToString(); } }
    public string TitleBarContent { get { return key.TitleBarContent; } }


    public WorkInfoData(WorkInfoKey key)
    {
        this.key = key;
    }
}
