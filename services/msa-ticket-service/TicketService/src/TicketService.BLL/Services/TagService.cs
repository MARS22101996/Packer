using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TicketService.BLL.DTO;
using TicketService.BLL.Interfaces;
using TicketService.DAL.Entities;
using TicketService.DAL.Interfaces;

namespace TicketService.BLL.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TagService> _logger;

        public TagService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TagService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public Task AddAsync(IEnumerable<string> tags)
        {
            var allTags = GetAll().ToList();
            var tasks = new List<Task>();

            foreach (var tag in tags)
            {
                if (allTags.All(t => t.Name != tag))
                {
                    var createTask = _unitOfWork.Tags.CreateAsync(new Tag { Name = tag });
                    tasks.Add(createTask);

                    _logger.LogInformation($"Tag with name {tag} was successfully created");
                }
            }

            return Task.WhenAll(tasks.ToArray());
        }

        public IEnumerable<TagDto> GetAll()
        {
            var tags = _unitOfWork.Tags.GetAll();
            var tagDtos = _mapper.Map<List<TagDto>>(tags);

            _logger.LogInformation("Tags were successfully got");

            return tagDtos;
        }
    }
}