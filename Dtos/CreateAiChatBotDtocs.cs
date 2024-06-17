namespace Ma3ak.Dtos
{
    public class CreateAiChatBotDtocs
    {
        public int UserId { get; set; }

        [MaxLength(10)]
        public string ConversationTimeSender { get; set; }
        [MaxLength(10)]
        public string ConversationTimeResever { get; set; }
        public string ConversationTextSender { get; set; }
        public string ConversationTextResever { get; set; }
        public IFormFile? Userpictures { get; set; }
        public IFormFile? Aipictures { get; set; }
        public bool isDeleted { get; set; }
    }
}
