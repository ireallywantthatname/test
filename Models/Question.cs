namespace Test.Data
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public string Difficulty { get; set; } = null!;
        public int PaperId { get; set; }
        public virtual Paper Paper { get; set; } = null!;
    }
}