namespace UnityEngine.UI
{
    /// <summary>
    /// author:zhouzhanglin
    /// </summary>
    public interface IPage
    {
		bool PageIsLoop();
        void ShowPage(int index);
    }
}
