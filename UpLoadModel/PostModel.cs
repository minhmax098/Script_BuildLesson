using System;
using Newtonsoft.Json;

public class PostModel
{
    [JsonProperty("code")]
    public long Code { get; set; }

    [JsonProperty("message")] public string Message { get; set; }

    [JsonProperty("data")] public Datum[] Data { get; set; }
}

public class Datum
{
    [JsonProperty("type")] public long Type { get; set; }

    [JsonProperty("extension")] public string Extension { get; set; }

    [JsonProperty("size")] public long Size { get; set; }

    [JsonProperty("file_name")] public string FileName { get; set; }

    [JsonProperty("file_path")] public string FilePath { get; set; }

    [JsonProperty("created_by")] public long CreatedBy { get; set; }

    [JsonProperty("created_date")] public DateTimeOffset CreatedDate { get; set; }

    [JsonProperty("file_id")] public long FileId { get; set; }
}