namespace TestApi.DAL.Helpers
{
    public class Integer
    {
        public Integer(int value)
        {
            Value = value;
        }

        public int Value { get; set; }

        public static implicit operator Integer(int x)
        {
            return new Integer(x);
        }

        public static implicit operator int(Integer x)
        {
            return x.Value;
        }
    }
}