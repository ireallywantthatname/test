namespace Test.Types
{
    public struct Choice
    {
        public string Title { get; set; }
        public string Details { get; set; }
        public Action EventHandler { get; set; }
    }
}