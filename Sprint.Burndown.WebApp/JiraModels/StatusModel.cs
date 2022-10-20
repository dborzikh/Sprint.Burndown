namespace Sprint.Burndown.WebApp.JiraModels
{
    public class StatusModel : JiraModelBase
    {
        public string Description { get; set; }

        public string IconUrl { get; set; }

        public StatusCategoryModel StatusCategory { get; set; }
    }
}
