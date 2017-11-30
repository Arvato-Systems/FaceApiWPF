namespace FaceApiWFP
{
    using Newtonsoft.Json;

    public partial class FaceModel
    {
        [JsonProperty("faceAttributes")]
        public FaceAttributes FaceAttributes { get; set; }

        [JsonProperty("faceId")]
        public string FaceId { get; set; }

        [JsonProperty("faceRectangle")]
        public FaceRectangle FaceRectangle { get; set; }
    }

    public partial class FaceRectangle
    {
        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("left")]
        public long Left { get; set; }

        [JsonProperty("top")]
        public long Top { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }
    }

    public partial class FaceAttributes
    {
        [JsonProperty("accessories")]
        public object[] Accessories { get; set; }

        [JsonProperty("age")]
        public double Age { get; set; }

        [JsonProperty("blur")]
        public Blur Blur { get; set; }

        [JsonProperty("emotion")]
        public Emotion Emotion { get; set; }

        [JsonProperty("exposure")]
        public Exposure Exposure { get; set; }

        [JsonProperty("facialHair")]
        public FacialHair FacialHair { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("glasses")]
        public string Glasses { get; set; }

        [JsonProperty("hair")]
        public Hair Hair { get; set; }

        [JsonProperty("headPose")]
        public HeadPose HeadPose { get; set; }

        [JsonProperty("makeup")]
        public Makeup Makeup { get; set; }

        [JsonProperty("noise")]
        public Noise Noise { get; set; }

        [JsonProperty("occlusion")]
        public Occlusion Occlusion { get; set; }

        [JsonProperty("smile")]
        public double Smile { get; set; }
    }

    public partial class Occlusion
    {
        [JsonProperty("eyeOccluded")]
        public bool EyeOccluded { get; set; }

        [JsonProperty("foreheadOccluded")]
        public bool ForeheadOccluded { get; set; }

        [JsonProperty("mouthOccluded")]
        public bool MouthOccluded { get; set; }
    }

    public partial class Noise
    {
        [JsonProperty("noiseLevel")]
        public string NoiseLevel { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }

    public partial class Makeup
    {
        [JsonProperty("eyeMakeup")]
        public bool EyeMakeup { get; set; }

        [JsonProperty("lipMakeup")]
        public bool LipMakeup { get; set; }
    }

    public partial class HeadPose
    {
        [JsonProperty("pitch")]
        public double Pitch { get; set; }

        [JsonProperty("roll")]
        public double Roll { get; set; }

        [JsonProperty("yaw")]
        public double Yaw { get; set; }
    }

    public partial class Hair
    {
        [JsonProperty("bald")]
        public double Bald { get; set; }

        [JsonProperty("hairColor")]
        public HairColor[] HairColor { get; set; }

        [JsonProperty("invisible")]
        public bool Invisible { get; set; }
    }

    public partial class HairColor
    {
        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }

    public partial class FacialHair
    {
        [JsonProperty("beard")]
        public double Beard { get; set; }

        [JsonProperty("moustache")]
        public double Moustache { get; set; }

        [JsonProperty("sideburns")]
        public double Sideburns { get; set; }
    }

    public partial class Exposure
    {
        [JsonProperty("exposureLevel")]
        public string ExposureLevel { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }

    public partial class Emotion
    {
        [JsonProperty("anger")]
        public double Anger { get; set; }

        [JsonProperty("contempt")]
        public double Contempt { get; set; }

        [JsonProperty("disgust")]
        public double Disgust { get; set; }

        [JsonProperty("fear")]
        public double Fear { get; set; }

        [JsonProperty("happiness")]
        public double Happiness { get; set; }

        [JsonProperty("neutral")]
        public double Neutral { get; set; }

        [JsonProperty("sadness")]
        public double Sadness { get; set; }

        [JsonProperty("surprise")]
        public double Surprise { get; set; }
    }

    public partial class Blur
    {
        [JsonProperty("blurLevel")]
        public string BlurLevel { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }

    public partial class FaceModel
    {
        public static FaceModel[] FromJson(string json) => JsonConvert.DeserializeObject<FaceModel[]>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this FaceModel[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
