public class IndexOptions
{
    public string Databases { get; set; } = null;
    public string FragmentationLow { get; set; } = null;
    public string FragmentationMedium { get; set; } = "INDEX_REORGANIZE,INDEX_REBUILD_ONLINE,INDEX_REBUILD_OFFLINE";
    public string FragmentationHigh { get; set; } = "INDEX_REBUILD_ONLINE,INDEX_REBUILD_OFFLINE";
    public int FragmentationLevel1 { get; set; } = 5;
    public int FragmentationLevel2 { get; set; } = 30;
    public int? MinNumberOfPages { get; set; } = 1000;
    public int? MaxNumberOfPages { get; set; } = null;
    public string SortInTempdb { get; set; } = "N";
    public int? MaxDOP { get; set; } = null;
    public int? FillFactor { get; set; } = null;
    public string PadIndex { get; set; } = null;
    public string LOBCompaction { get; set; } = "Y";
    public string UpdateStatistics { get; set; } = null;
    public string OnlyModifiedStatistics { get; set; } = "N";
    public int? StatisticsModificationLevel { get; set; } = null;
    public int? StatisticsSample { get; set; } = null;
    public string StatisticsResample { get; set; } = "N";
    public string PartitionLevel { get; set; } = "Y";
    public string MSShippedObjects { get; set; } = "N";
    public string Indexes { get; set; } = null;
    public int? TimeLimit { get; set; } = null;
    public int? Delay { get; set; } = null;
    public int? WaitAtLowPriorityMaxDuration { get; set; } = null;
    public string WaitAtLowPriorityAbortAfterWait { get; set; } = null;
    public string Resumable { get; set; } = "N";
    public string AvailabilityGroups { get; set; } = null;
    public int? LockTimeout { get; set; } = null;
    public int LockMessageSeverity { get; set; } = 16;
    public string StringDelimiter { get; set; } = ",";
    public string DatabaseOrder { get; set; } = null;
    public string DatabasesInParallel { get; set; } = "N";
    public string ExecuteAsUser { get; set; } = null;
    public string LogToTable { get; set; } = "N";
    public string Execute { get; set; } = "Y";
}
