namespace FaceApiWFP
{
    public partial class MainWindow
    {
        struct EmotionElement
        {
            public string EmotionName { get; private set; }
            public double EmotionValue { get; private set; }
            public EmotionElement(string emotionName, double emotionValue)
            {
                EmotionName = emotionName;
                EmotionValue = emotionValue;
            }
        }
    }
}
