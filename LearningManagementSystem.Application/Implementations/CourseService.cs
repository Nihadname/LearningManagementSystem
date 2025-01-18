﻿using AutoMapper;
using LearningManagementSystem.Application.Dtos.Course;
using LearningManagementSystem.Application.Dtos.Note;
using LearningManagementSystem.Application.Exceptions;
using LearningManagementSystem.Application.Interfaces;
using LearningManagementSystem.Core.Entities;
using LearningManagementSystem.Core.Entities.Common;
using LearningManagementSystem.DataAccess.Data.Implementations;
using Microsoft.EntityFrameworkCore;

namespace LearningManagementSystem.Application.Implementations
{
    public class CourseService:ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoOrVideoService _photoOrVideoService;
        public CourseService(IUnitOfWork unitOfWork, IMapper mapper, IPhotoOrVideoService photoOrVideoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoOrVideoService = photoOrVideoService;
        }
        public async Task<Result<CourseCreateOrUpdateReturnDto>> Create(CourseCreateDto courseCreateDto)
        {
            if(await _unitOfWork.CourseRepository.isExists(s=>s.Name.ToLower() == courseCreateDto.Name.ToLower()))
            {
                return Result<CourseCreateOrUpdateReturnDto>.Failure("Name", "There is an already existed course",ErrorType.BusinessLogicError);
            }
            
           var MappedCourse=_mapper.Map<Course>(courseCreateDto);
            MappedCourse.ImageUrl = await _photoOrVideoService.UploadMediaAsync(courseCreateDto.formFile, false);
            await _unitOfWork.CourseRepository.Create(MappedCourse);
            await _unitOfWork.Commit();
            var ResponseCourseDto=_mapper.Map<CourseCreateOrUpdateReturnDto>(MappedCourse);
            return Result<CourseCreateOrUpdateReturnDto>.Success(ResponseCourseDto);
        }
        public async  Task<Result<CourseReturnDto>> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
               return Result<CourseReturnDto>.Failure(null, "Invalid GUID provided.",ErrorType.ValidationError);
            }
            var ExistedCourse=await _unitOfWork.CourseRepository.GetEntity(s=>s.Id==id&& s.IsDeleted == false,true, includes: new Func<IQueryable<Course>, IQueryable<Course>>[] {
                 query => query
            .Include(p => p.lessons)
            });
            if (ExistedCourse is null)
            {
                return Result<CourseReturnDto>.Failure("Course", "Not found", ErrorType.NotFoundError);
            }
            var MappedCourse = _mapper.Map<CourseReturnDto>(ExistedCourse);
            return Result<CourseReturnDto>.Success(MappedCourse);
        }
        public async Task<Result<CourseCreateOrUpdateReturnDto>> Update(Guid id,CourseUpdateDto courseUpdateDto)
        {
            if (id == Guid.Empty)
            {
              return  Result<CourseCreateOrUpdateReturnDto>.Failure(null, "Invalid GUID provided.", ErrorType.ValidationError);
            }
            if (courseUpdateDto.Name != null)
            {
                if (await _unitOfWork.CourseRepository.isExists(s => s.Name.ToLower() == courseUpdateDto.Name.ToLower()))
                {
                 return   Result<CourseCreateOrUpdateReturnDto>.Failure("Name", "There is an already existed course", ErrorType.BusinessLogicError);
                }
            }
            var ExistedCourse = await _unitOfWork.CourseRepository.GetEntity(s =>s.Id == id);
            if(ExistedCourse == null)
            {
             return   Result<CourseCreateOrUpdateReturnDto>.Failure(null, "Course doesnt exist", ErrorType.NotFoundError);
            }
            _mapper.Map(courseUpdateDto, ExistedCourse);
            await _unitOfWork.CourseRepository.Update(ExistedCourse);
            await _unitOfWork.Commit();
            var ResponseCourseDto = _mapper.Map<CourseCreateOrUpdateReturnDto>(ExistedCourse);
            return Result< CourseCreateOrUpdateReturnDto>.Success(ResponseCourseDto);
        }
        public async Task<List<CourseSelectItemDto>> GetAllAsSelectItem()
        {
            var allCourses= await _unitOfWork.CourseRepository.GetAll(s=>s.IsDeleted == false, true);
            var MappedCourses=  _mapper.Map<List<CourseSelectItemDto>>(allCourses);
            return MappedCourses;
        }
        public async Task<Result<string>> DeleteFromUi(Guid id)
        {
            var existedCourseResult = await GetCourseById(id);
            var existedCourse = existedCourseResult.Data;
            if (existedCourse.IsDeleted is true) Result<string>.Failure("Course", "this already existed",ErrorType.BusinessLogicError);
            existedCourse.IsDeleted = true;
            await _unitOfWork.Commit();
            return Result<string>.Success("Deleted");
        }
        public async Task<Result<string>> Delete(Guid id)
        {
            var existedCourseResult = await GetCourseById(id);
            var existedCourse=existedCourseResult.Data;
      await _unitOfWork.CourseRepository.Delete(existedCourse);
            await _unitOfWork.Commit();
            return Result<string>.Success("Deleted");
        }
        public async Task<Result<PaginationDto<CourseListItemDto>>> GetAll(List<Guid> TeacherIds, int pageNumber = 1,
           int pageSize = 10, 
           string searchQuery = null)
        {
            var courseQuery = await _unitOfWork.CourseRepository.GetQuery(s => s.IsDeleted == false,true,true,  includes: new Func<IQueryable<Course>, IQueryable<Course>>[] {
                 query => query
            .Include(p => p.lessons)
            });
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                courseQuery = courseQuery.Where(s => s.Name.Contains(searchQuery) || s.Description.Contains(searchQuery));
            }
            if(TeacherIds.Any())
            {
                foreach (var TeacherId in TeacherIds)
                {
                    if (!await _unitOfWork.TeacherRepository.isExists(s => s.Id == TeacherId))
                       return Result<PaginationDto<CourseListItemDto>>.Failure("TeacherId", "TeacherId is invalid", ErrorType.NotFoundError);
                    courseQuery = courseQuery.Where(s => s.lessons.Any(s => s.TeacherId == TeacherId));
                }
            }


            var mappedNotes = _mapper.Map<IQueryable<CourseListItemDto>>(courseQuery);

            var paginationResult = await PaginationDto<CourseListItemDto>.Create(mappedNotes, pageNumber, pageSize);

            return Result<PaginationDto<CourseListItemDto>>.Success(paginationResult);
        }
        private async Task<Result<Course>> GetCourseById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Result<Course>.Failure(null, "Invalid GUID provided.", ErrorType.ValidationError);
            }
            var ExistedCourse = await _unitOfWork.CourseRepository.GetEntity(s => s.Id == id && s.IsDeleted == false,true);
            if (ExistedCourse is null)
            {
                return Result<Course>.Failure("Course", "Not found", ErrorType.NotFoundError);
            }
            return Result<Course>.Success(ExistedCourse);
        }
    }
}
