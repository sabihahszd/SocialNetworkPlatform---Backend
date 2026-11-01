using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class ChatThreadService : IChatThreadService
    {
        private readonly IChatThreadRepository _chatThreadRepository;

        public ChatThreadService(IChatThreadRepository chatThreadRepository)
        {
            _chatThreadRepository = chatThreadRepository;
        }

        public Task CreateAsync(ChatThread entity)
        {
            return _chatThreadRepository.CreateAsync(entity);
        }

        public Task DeleteAsync(ChatThread entity)
        {
            return _chatThreadRepository.DeleteAsync(entity);
        }

        public Task EditAsync(ChatThread entity)
        {
            return _chatThreadRepository.EditAsync(entity);
        }

        public Task<ChatThread?> GetByIdAsync(int id)
        {
            return _chatThreadRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<ChatThread>> GetAllAsync()
        {
            return _chatThreadRepository.GetAllAsync();
        }

        public Task SaveAsync()
        {
            return _chatThreadRepository.SaveAsync();
        }

        public Task<ChatThread?> GetByPublicIdAsync(string publicId)
        {
            return _chatThreadRepository.GetByPublicIdAsync(publicId);
        }

        public Task<ChatThread?> GetBetweenAsync(int userId, int otherId)
        {
            return _chatThreadRepository.GetBetweenAsync(userId, otherId);
        }

        public Task<List<ChatThread>> GetForUserAsync(int userId)
        {
            return _chatThreadRepository.GetForUserAsync(userId);
        }
    }
}
