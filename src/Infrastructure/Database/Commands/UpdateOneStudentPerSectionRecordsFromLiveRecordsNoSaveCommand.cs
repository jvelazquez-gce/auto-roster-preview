using Domain.Models.Helper;
using System.Linq;
using Domain.Constants;
using System;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Commands
{
    public class UpdateOneStudentPerSectionRecordsFromLiveRecordsNoSaveCommand : IUpdateOneStudentPerSectionRecordsFromLiveRecordsNoSaveCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public UpdateOneStudentPerSectionRecordsFromLiveRecordsNoSaveCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public OneStudentPerSectionResults ExecuteCommand(OneStudentPerSectionResults recordsToUpdate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            foreach (var liveRec in recordsToUpdate.LiveStudentSectionList)
            {
                var recToUpdate = recordsToUpdate.OneStudentPerSectionList
                                    .Where(w => w.SectionGuid == liveRec.SectionGuid)
                                    .Where(w => w.SyStudentID == liveRec.SyStudentID)
                                    .Where(w => w.OldSectionId == liveRec.OldSectionId)
                                    .Where(w => w.GroupCategory == ARBGroupCategory.ONE_STUDENT_PER_SECTION)
                                    .FirstOrDefault();

                if (recToUpdate != null)
                {
                    recToUpdate.StatusID = liveRec.StatusID;
                    recToUpdate.UpdatedBy = "ARB Live Job";
                    recToUpdate.UpdatedOn = DateTime.Now;
                    context.Entry(recToUpdate).State = EntityState.Modified;
                }
            }

            return recordsToUpdate;
        }
    }
}
