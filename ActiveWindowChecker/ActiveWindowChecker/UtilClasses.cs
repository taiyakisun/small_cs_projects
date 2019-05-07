using System;
using System.Collections.Generic;
using System.ComponentModel;


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


/// <summary>
/// Provides a generic collection that supports data binding and additionally supports sorting.
/// See http://msdn.microsoft.com/en-us/library/ms993236.aspx
/// If the elements are IComparable it uses that; otherwise compares the ToString()
/// 
/// The following code is from http://martinwilley.com/net/code/forms/sortablebindinglist.html
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public class SortableBindingList<T> : BindingList<T> where T : class
{
    private bool _isSorted;
    private ListSortDirection _sortDirection = ListSortDirection.Ascending;
    private PropertyDescriptor _sortProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableBindingList{T}"/> class.
    /// </summary>
    public SortableBindingList()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableBindingList{T}"/> class.
    /// </summary>
    /// <param name="list">An <see cref="T:System.Collections.Generic.IList`1" /> of items to be contained in the <see cref="T:System.ComponentModel.BindingList`1" />.</param>
    public SortableBindingList(IList<T> list)
        : base(list)
    {
    }

    /// <summary>
    /// Gets a value indicating whether the list supports sorting.
    /// </summary>
    protected override bool SupportsSortingCore
    {
        get { return true; }
    }

    /// <summary>
    /// Gets a value indicating whether the list is sorted.
    /// </summary>
    protected override bool IsSortedCore
    {
        get { return _isSorted; }
    }

    /// <summary>
    /// Gets the direction the list is sorted.
    /// </summary>
    protected override ListSortDirection SortDirectionCore
    {
        get { return _sortDirection; }
    }

    /// <summary>
    /// Gets the property descriptor that is used for sorting the list if sorting is implemented in a derived class; otherwise, returns null
    /// </summary>
    protected override PropertyDescriptor SortPropertyCore
    {
        get { return _sortProperty; }
    }

    /// <summary>
    /// Removes any sort applied with ApplySortCore if sorting is implemented
    /// </summary>
    protected override void RemoveSortCore()
    {
        _sortDirection = ListSortDirection.Ascending;
        _sortProperty = null;
        _isSorted = false; //thanks Luca
    }

    /// <summary>
    /// Sorts the items if overridden in a derived class
    /// </summary>
    /// <param name="prop"></param>
    /// <param name="direction"></param>
    protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
    {
        _sortProperty = prop;
        _sortDirection = direction;

        List<T> list = Items as List<T>;
        if (list == null) return;

        list.Sort(Compare);

        _isSorted = true;
        //fire an event that the list has been changed.
        OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }


    private int Compare(T lhs, T rhs)
    {
        var result = OnComparison(lhs, rhs);
        //invert if descending
        if (_sortDirection == ListSortDirection.Descending)
            result = -result;
        return result;
    }

    private int OnComparison(T lhs, T rhs)
    {
        object lhsValue = lhs == null ? null : _sortProperty.GetValue(lhs);
        object rhsValue = rhs == null ? null : _sortProperty.GetValue(rhs);
        if (lhsValue == null)
        {
            return (rhsValue == null) ? 0 : -1; //nulls are equal
        }
        if (rhsValue == null)
        {
            return 1; //first has value, second doesn't
        }
        if (lhsValue is IComparable)
        {
            return ((IComparable)lhsValue).CompareTo(rhsValue);
        }
        if (lhsValue.Equals(rhsValue))
        {
            return 0; //both are the same
        }
        //not comparable, compare ToString
        return lhsValue.ToString().CompareTo(rhsValue.ToString());
    }
}
