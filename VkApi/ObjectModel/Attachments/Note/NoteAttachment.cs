using System.Text.Json.Serialization;

namespace VkApi.ObjectModel.Attachments.Note;

public class NoteAttachment : IAttachmentElement
{
    public AttachmentElementType Type => AttachmentElementType.Note;
    
    [JsonPropertyName("id")] 
    public int Id { get; set; }
    
    [JsonPropertyName("owner_id")] 
    public int OwnerId { get; set; }
    
    [JsonPropertyName("title")] 
    public string Title { get; set; }
    
    [JsonPropertyName("text")] 
    public string Text { get; set; }
    
    [JsonPropertyName("access_key")] 
    public string AccessKey { get; set; }
}