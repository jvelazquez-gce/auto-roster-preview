using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Models.Helper;
using Domain.Entities;
using Infrastructure.RulesEngine;
using Newtonsoft.Json;
using AutoMapper;

namespace Services.RulesEngine.GroupedStudents
{
    public class GroupOrganizer : IGroupOrganizer
    {
        private List<StudentSectionError> _errorPreLoadList = new List<StudentSectionError>();
        private List<PreLoadStudentSection> _preLoadStudentSections = new List<PreLoadStudentSection>();
        private readonly IMapper _mapper;

        public GroupOrganizer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<PreLoadStudentSection> GetListWithUpdateGroupDetails(List<PreLoadStudentSection> preLoadStudentSections, Parameters p, List<Rule> rules)
        {
            var recordsToUpdate = preLoadStudentSections
                .Where(pre => pre.SectionCode.Equals(GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER, StringComparison.OrdinalIgnoreCase))
                .Where(pre => pre.GroupTypeKey == null || pre.GroupTypeKey == 0)
                .Where(pre => pre.GroupNumber == null || pre.GroupNumber == Guid.Empty)
                .ToList();

            var errors = new List<PreLoadStudentSection>();
            var groupNumber = Guid.NewGuid();
            foreach (var preLoadStudentSection in recordsToUpdate)
            {
                var allRulesValidated = true;
                var errorFound = false;
                foreach (var rule in rules)
                {
                    try
                    {
                        if (!DynamicValueHandler.IsRuleValid(preLoadStudentSection, rule)) allRulesValidated = false;
                    }
                    catch (Exception ex)
                    {
                        errorFound = true;
                        var serializedRule = JsonConvert.SerializeObject(rule);
                        var serializedPreload = JsonConvert.SerializeObject(preLoadStudentSection);
                        var errmsg = $"{serializedRule} - {serializedPreload} - {ex}";
                        errors.Add(preLoadStudentSection);
                        preLoadStudentSections.Remove(preLoadStudentSection);
                        AddErrorRecord(preLoadStudentSection, RulesEngineConstants.Error_Rule_Not_Met, p.Job.Id, errmsg);
                        preLoadStudentSections.Remove(preLoadStudentSection);
                    }
                }

                if(allRulesValidated)
                {
                    preLoadStudentSection.GroupNumber = groupNumber;
                    preLoadStudentSection.GroupTypeKey = Group.IN_RULE_GROUP;
                }
                else if(!errorFound)
                {
                    preLoadStudentSection.GroupTypeKey = Group.NOT_IN_RULE_GROUP;
                }
                _preLoadStudentSections.Add(preLoadStudentSection);
            }

            if (_errorPreLoadList.Any()) p.AddStudentSectionErrorCommand.ExecuteCommand(_errorPreLoadList);

            return _preLoadStudentSections;
        }

        private void AddErrorRecord(PreLoadStudentSection preLoadStudentSections, int statusId, int jobId, string errMsg)
        {
            var sectionError = new StudentSectionError();
            _mapper.Map<PreLoadStudentSection, StudentSectionError>(preLoadStudentSections, sectionError);
            sectionError.StatusID = statusId;
            sectionError.JobID = jobId;
            sectionError.ErrorMessage = errMsg;
            _errorPreLoadList.Add(sectionError);
        }
    }
}
