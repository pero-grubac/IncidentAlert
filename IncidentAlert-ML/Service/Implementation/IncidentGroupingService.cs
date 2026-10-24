using IncidentAlert_ML.Model;
using Microsoft.ML;

namespace IncidentAlert_ML.Service.Implementation
{
    public class IncidentGroupingService : IIncidentGroupingService
    {
        public const int MaxClusters = 5;

        public Task<List<IncidentGroup>> GroupIncidentsByText(List<SimpleIncident> incidents)
        {
            // KMeans cannot build more clusters than there are rows.
            if (incidents.Count == 0)
                return Task.FromResult(new List<IncidentGroup>());

            // A fixed seed keeps the grouping stable between requests for the same data.
            var mlContext = new MLContext(seed: 1);

            // Step 1: Project the data to only include the `Text` field
            var textData = incidents.Select(incident => new { Text = incident.Text }).ToList();

            // Step 2: Load data into an IDataView, only with the `Text` column
            var data = mlContext.Data.LoadFromEnumerable(textData);

            // Step 3: Configure Text Featurizer (Transform text into numeric vectors)
            var textPipeline = mlContext.Transforms.Text.FeaturizeText("Features", "Text");

            // Step 4: Transform the data
            var textTransformer = textPipeline.Fit(data);
            var transformedData = textTransformer.Transform(data);

            // Step 5: Use KMeans clustering to group similar texts
            var options = new Microsoft.ML.Trainers.KMeansTrainer.Options
            {
                NumberOfClusters = Math.Min(MaxClusters, incidents.Count),
                FeatureColumnName = "Features"
            };
            var trainer = mlContext.Clustering.Trainers.KMeans(options);
            var model = trainer.Fit(transformedData);

            // Step 6: Predict clusters for each incident
            var predictions = model.Transform(transformedData);
            var predictionResults = mlContext.Data.CreateEnumerable<IncidentPrediction>(predictions, reuseRowObject: false).ToList();

            // Step 7: Group incidents based on their cluster assignments
            var groupedIncidents = new List<IncidentGroup>();
            for (int i = 0; i < predictionResults.Count; i++)
            {
                var clusterId = predictionResults[i].PredictedClusterId;
                if (groupedIncidents.All(g => g.GroupKey != $"Cluster {clusterId}"))
                {
                    groupedIncidents.Add(new IncidentGroup { GroupKey = $"Cluster {clusterId}", Incidents = new List<SimpleIncident>() });
                }
                groupedIncidents.First(g => g.GroupKey == $"Cluster {clusterId}").Incidents.Add(incidents[i]);
            }

            return Task.FromResult(groupedIncidents);
        }

    }
}
