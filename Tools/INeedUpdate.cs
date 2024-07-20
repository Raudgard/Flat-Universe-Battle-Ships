
namespace Tools
{
    /// <summary>
    /// ���������, ��� ����������������� ���������� Update ��� ���� ������������������ ��������.
    /// ����������� ���������������� ������ � ���� ����������� � Updater ����� Register � Unregister.
    /// </summary>
    public interface INeedUpdate
    {
        /// <summary>
        /// Execute every frame.
        /// </summary>
        public void UpdateMe();
    }
}
