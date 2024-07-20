
namespace Tools
{
    /// <summary>
    /// ��������������� �����, ���������� ���� ��� ������������� �������� ����. ��� ������������� � ��������� ����� ������ �������.
    /// </summary>
    /// <typeparam name="Tkey"></typeparam>
    /// <typeparam name="Tvalue"></typeparam>
    [System.Serializable]
    public class TwoValuePair<Tkey, Tvalue>
    {
        public Tkey key;
        public Tvalue value;

        public TwoValuePair(Tkey tkey, Tvalue tvalue)
        {
            key = tkey;
            value = tvalue;
        }
    }
}