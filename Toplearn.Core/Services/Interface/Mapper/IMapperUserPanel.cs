using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.UserPanel;
using Toplearn.Core.DTOs.Wallet;
using Toplearn.DataLayer.Entities.Wallet;

namespace Toplearn.Core.Services.Interface.Mapper
{
    public interface IMapperUserPanel
    {
        public UserPanelViewModel MapTheUserPanelViewModelFromClaims(List<Claim> claims);

        public EditPanelViewModel MapTheEditPanelViewModelFromClaims(IEnumerable<Claim> claims);

    }
}
