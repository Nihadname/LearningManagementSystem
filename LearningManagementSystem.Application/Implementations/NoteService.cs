﻿using AutoMapper;
using LearningManagementSystem.Application.Dtos.Note;
using LearningManagementSystem.Application.Dtos.Paganation;
using LearningManagementSystem.Application.Exceptions;
using LearningManagementSystem.Application.Interfaces;
using LearningManagementSystem.Core.Entities;
using LearningManagementSystem.DataAccess.Data.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Application.Implementations
{
    public class NoteService:INoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NoteService(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<NoteReturnDto> Create(NoteCreateDto noteCreateDto)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new CustomException(400, "Id", "User ID cannot be null");
            }
            var existedUser = await _userManager.Users
     .Include(u => u.Notes)
     .FirstOrDefaultAsync(u => u.Id == userId);
            if (existedUser == null)
            {
                throw new CustomException(400, "User", "User  cannot be null");
            }
            if (existedUser.Notes.Any(s => s.Title.Equals(noteCreateDto.Title, StringComparison.OrdinalIgnoreCase)))
            {
                throw new CustomException(400, "Title", "User already has Title like this");
            }
            noteCreateDto.AppUserId = userId;
            var MappedNote = _mapper.Map<Note>(noteCreateDto);
            await _unitOfWork.NoteRepository.Create(MappedNote);
            await _unitOfWork.Commit();
            var MappedResponse=_mapper.Map<NoteReturnDto>(MappedNote);
            return MappedResponse;
        }
        public async Task<PaginationDto<NoteListItemDto>> GetAll(int pageNumber = 1,
           int pageSize = 10,
           string searchQuery = null)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new CustomException(400, "Id", "User ID cannot be null");
            }
           
            var notesQuery = await _unitOfWork.NoteRepository.GetQuery(s=>s.AppUserId==userId&&s.IsDeleted==false);
            if(!string.IsNullOrWhiteSpace(searchQuery))
            {
                notesQuery= notesQuery.Where(s => s.Title.Contains(searchQuery) || s.Description.Contains(searchQuery));
            }

            var totalCount = await notesQuery.CountAsync();

            var paginatedQuery = notesQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var notesList = await paginatedQuery.ToListAsync();
            var mappedNotes = _mapper.Map<List<NoteListItemDto>>(notesList);

            var paginationResult = await PaginationDto<NoteListItemDto>.Create(mappedNotes, pageNumber, pageSize, totalCount);

            return paginationResult;
        }
        public async Task<string> DeleteForUser(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                throw new CustomException(440, "Invalid GUID provided.");
            }
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new CustomException(400, "Id", "User ID cannot be null");
            }
            var existedNote = await _unitOfWork.NoteRepository.GetEntity(s => s.IsDeleted == false && s.AppUserId == userId && s.Id == Id);
            if (existedNote == null)
            {
                throw new CustomException(404, "Note", "Note not found");
            }
            await _unitOfWork.NoteRepository.Delete(existedNote);
            await _unitOfWork.Commit();
            return "succesfully deleted";
        }
    }
}