﻿using AutoMapper;
using LearningManagementSystem.Application.Dtos.Ai;
using LearningManagementSystem.Application.Dtos.RequstToRegister;
using LearningManagementSystem.Application.Exceptions;
using LearningManagementSystem.Application.Interfaces;
using LearningManagementSystem.Core.Entities;
using LearningManagementSystem.DataAccess.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Application.Implementations
{
    public class RequstToRegisterService : IRequstToRegisterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RequstToRegisterService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<string> Create(RequstToRegisterCreateDto requstToRegisterCreateDto)
        {

            if (!await _unitOfWork.CourseRepository.isExists(s => s.Id == requstToRegisterCreateDto.ChoosenCourse))
            {
                throw new CustomException(400, "Course", "You have choosen an invalid course");
            }
            List<string> allCoursesNames = (await _unitOfWork.CourseRepository.GetAll()).Select(s => s.Name).ToList();
            //   requstToRegisterCreateDto.ExistedCourses = allCoursesNames;
            if (requstToRegisterCreateDto.IsParent is true)
            {
                if (string.IsNullOrWhiteSpace(requstToRegisterCreateDto.ChildName))
                {
                    throw new CustomException(400, "Parent", "You identify as a parent so , you have to mention name of your child");

                }
                if (requstToRegisterCreateDto.ChildAge is null || !requstToRegisterCreateDto.ChildAge.HasValue)
                {
                    throw new CustomException(400, "Parent", "You identify as a parent so , you have to mention age of your child");
                }
                var existedCourse = await _unitOfWork.CourseRepository.GetEntity(s=>s.Id==requstToRegisterCreateDto.ChoosenCourse);
                if (existedCourse is null)
                {
                    throw new CustomException(400, "Course", "this doesnt exist");
                }
                requstToRegisterCreateDto.AiResponse = await GetAdviceFromAi(existedCourse, requstToRegisterCreateDto.ChildName, (int)requstToRegisterCreateDto.ChildAge);
            }
            var MappedRequestRegister = _mapper.Map<RequestToRegister>(requstToRegisterCreateDto);
            await _unitOfWork.RequstToRegisterRepository.Create(MappedRequestRegister);
            await _unitOfWork.Commit();


            return MappedRequestRegister.AiResponse;

        }
        private async Task<string> GetAdviceFromAi(Course course, string childName, int childAge)
        {
            var apiKey = "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";
            var url = "https://api.openai.com/v1/chat/completions";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var prompt = $@"
A guardian is registering for the course '{course.Name}' for a {childAge}-year-old child.
Course Description: {course.Description}.
Provide advice for the registration.";


                var requestPayload = new
                {
                    model = "gpt-4",
                    messages = new[]
                    {
                new { role = "system", content = "You are a helpful assistant providing course advice." },
                new { role = "user", content = prompt }
            }
                };

                var response = await client.PostAsJsonAsync(url, requestPayload);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
                    return result?.Choices?.FirstOrDefault()?.Message?.Content ?? "No advice available.";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return $"AI service is unavailable at the moment.";
                }
            }
        }

        

        

       

    }
}
