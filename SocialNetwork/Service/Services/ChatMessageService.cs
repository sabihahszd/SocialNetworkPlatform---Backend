using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IChatMessageRepository _chatMessageRepository;

        public ChatMessageService(IChatMessageRepository chatMessageRepository)
        {
            _chatMessageRepository = chatMessageRepository;
        }

        public Task CreateAsync(ChatMessage entity)
        {
            return _chatMessageRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(ChatMessage entity)
        {
            return _chatMessageRepository.DeleteAsync(entity);
        }

        public Task EditAsync(ChatMessage entity)
        {
            return _chatMessageRepository.EditAsync(entity);
        }

        public Task<ChatMessage?> GetByIdAsync(int id)
        {
            return _chatMessageRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<ChatMessage>> GetAllAsync()
        {
            return _chatMessageRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _chatMessageRepository.SaveAsync();
        }

        public Task<ChatMessage?> GetByPublicIdAsync(string publicId, int authorId)
        {
            return _chatMessageRepository.GetByPublicIdAsync(publicId, authorId);
        }
    }
}
