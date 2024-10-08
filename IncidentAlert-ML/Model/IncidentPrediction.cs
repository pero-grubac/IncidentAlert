using Microsoft.ML.Data;

namespace IncidentAlert_ML.Model
{
    public class IncidentPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId { get; set; }
        public float[] Features { get; set; }
    }
}
