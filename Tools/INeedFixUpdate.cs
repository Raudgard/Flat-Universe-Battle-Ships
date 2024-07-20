
namespace Tools
{
    /// <summary>
    /// ���������, ��� ����������������� ���������� Update ��� ���� ������������������ ��������.
    /// ����������� ���������������� ������ � ���� ����������� � Updater ����� Register � Unregister.
    /// </summary>
    public interface INeedFixUpdate
    {
        /// <summary>
        /// Execute every frame.
        /// </summary>
        public void FixUpdateMe();
    }
}
