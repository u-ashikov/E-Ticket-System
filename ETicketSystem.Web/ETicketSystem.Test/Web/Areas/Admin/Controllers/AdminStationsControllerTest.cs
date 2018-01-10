namespace ETicketSystem.Test.Web.Areas.Admin.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Common.Enums;
	using ETicketSystem.Services.Admin.Contracts;
	using ETicketSystem.Services.Admin.Models;
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Web.Areas.Admin.Controllers;
	using ETicketSystem.Web.Areas.Admin.Models.AdminStations;
	using FluentAssertions;
	using Infrastructure;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using Mocks;
	using Mocks.Admin;
	using Moq;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	using static Common.TestConstants;

	public class AdminStationsControllerTest : BaseControllerTest
    {
		private const int TotalStations = 30;

		private const int MaxPageSize = 2;

		private const string StationName = "Some station";

		private const int StationTownId = 1;

		private const string StationPhone = "0898978781";

		private const string Town = "Sofia";

		private const int EditStationId = 10;

		private readonly Mock<IAdminStationService> stationService = AdminStationServiceMock.New;

		private readonly Mock<ITownService> townService = TownServiceMock.New;

		[Fact]
		public void ControllerShouldBeForAuthorizedUsersOnly()
		{
			//Arrange
			var controller = new AdminStationsController(null,null);

			//Act
			var attributes = controller.GetType().GetCustomAttributes(true);

			//Assert
			attributes.Any(a => a.GetType() == typeof(AuthorizeAttribute));
		}

		[Fact]
		public void ControllerShouldBeForAdministratorsOnly()
		{
			//Arrange
			var controller = new AdminStationsController(null, null);

			//Act
			var attributes = controller.GetType().GetCustomAttributes(true);

			//Assert
			attributes.Any(a => a.GetType() == typeof(AuthorizeAttribute));
			var authorizeAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute)).As<AuthorizeAttribute>();
			authorizeAttribute.Roles.Should().Be(Role.Administrator.ToString());
		}

		[Fact]
		public void ControllerShouldBeInAdminArea()
		{
			//Arrange
			var controller = new AdminStationsController(null, null);

			//Act
			var attributes = controller.GetType().GetCustomAttributes(true);

			//Assert
			attributes.Any(a => a.GetType() == typeof(AreaAttribute));
			var areaAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AreaAttribute)).As<AreaAttribute>();
			areaAttribute.RouteValue.Should().Be(WebConstants.Area.Admin);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
		public void All_WithPageLessOrEqualToZeroShouldRedirectToAllStations(int page)
		{
			//Arrange
			var controller = new AdminStationsController(null, null);

			//Act
			var result = controller.All(string.Empty, page);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllStations);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
			model.RouteValues.Values.Should().Contain(MinPageSize);
			model.RouteValues.Keys.Should().Contain(RouteValueKeySearchTerm);
			model.RouteValues.Values.Should().Contain(string.Empty);
		}

		[Fact]
		public void All_ValidPageAndNoStationsShouldReturnCountZero()
		{
			//Arrange
			var controller = new AdminStationsController(null, this.stationService.Object);

			this.stationService.Setup(s => s.All(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(new List<AdminStationListingServiceModel>());

			this.stationService.Setup(s => s.TotalStations(It.IsAny<string>()))
				.Returns(0);

			//Act
			var result = controller.All(string.Empty, MinPageSize);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model.As<AllStations>();
			model.Stations.Should().HaveCount(0);
			model.Pagination.SearchTerm.Should().Be(string.Empty);
			model.Pagination.CurrentPage.Should().Be(MinPageSize);
			model.Pagination.NextPage.Should().Be(0);
			model.Pagination.PreviousPage.Should().Be(MinPageSize);
			model.Pagination.TotalElements.Should().Be(0);
		}

		[Fact]
		public void All_PageGreaterThanTotalPagesShouldRedirectToAllStations()
		{
			const int InvalidPageSize = 10;

			//Arrange
			var controller = new AdminStationsController(null, this.stationService.Object);

			this.stationService.Setup(s => s.All(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(this.GetStations());

			this.stationService.Setup(s => s.TotalStations(It.IsAny<string>()))
				.Returns(TotalStations);

			//Act
			var result = controller.All(string.Empty, InvalidPageSize);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllStations);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
			model.RouteValues.Values.Should().Contain(MaxPageSize);
			model.RouteValues.Keys.Should().Contain(RouteValueKeySearchTerm);
			model.RouteValues.Values.Should().Contain(string.Empty);
		}

		[Fact]
		public void All_ShouldReturnCorrectDataWithValidInput()
		{
			//Arrange
			var controller = new AdminStationsController(null, this.stationService.Object);

			this.stationService.Setup(s => s.All(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(this.GetStations());

			this.stationService.Setup(s => s.TotalStations(It.IsAny<string>()))
				.Returns(TotalStations);

			//Act
			var result = controller.All(string.Empty, MinPageSize);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model.As<AllStations>();
			model.Stations.Should().HaveCount(TotalStations);
			model.Pagination.SearchTerm.Should().Be(string.Empty);
			model.Pagination.CurrentPage.Should().Be(MinPageSize);
			model.Pagination.NextPage.Should().Be(MaxPageSize);
			model.Pagination.PreviousPage.Should().Be(MinPageSize);
			model.Pagination.TotalElements.Should().Be(TotalStations);
		}

		[Fact]
		public void All_ShouldReturnCorrectDataWithValidInputAndParticularSearchTerm()
		{
			const string SearchTerm = "Test";

			//Arrange
			var controller = new AdminStationsController(null, this.stationService.Object);

			this.stationService.Setup(s => s.All(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(this.GetStations());

			this.stationService.Setup(s => s.TotalStations(It.IsAny<string>()))
				.Returns(TotalStations);

			//Act
			var result = controller.All(SearchTerm, MinPageSize);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model.As<AllStations>();
			model.Stations.Should().HaveCount(TotalStations);
			model.Pagination.SearchTerm.Should().Be(SearchTerm);
			model.Pagination.CurrentPage.Should().Be(MinPageSize);
			model.Pagination.NextPage.Should().Be(MaxPageSize);
			model.Pagination.PreviousPage.Should().Be(MinPageSize);
			model.Pagination.TotalElements.Should().Be(TotalStations);
		}

		[Fact]
		public void Get_Add_ShouldReturnView()
		{
			//Arrange
			var controller = new AdminStationsController(this.townService.Object, this.stationService.Object);

			//Act
			var result = controller.Add();

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<StationFormModel>();
		}

		[Fact]
		public void Post_Add_WithInvalidModelStateShouldReturnView()
		{
			//Arrange
			var controller = new AdminStationsController(this.townService.Object, this.stationService.Object);
			controller.ModelState.AddModelError(string.Empty, "Error");

			//Act
			var result = controller.Add(this.GetAddStationFormModel());

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<StationFormModel>();
			var form = model.As<StationFormModel>();
			form.IsEdit.Should().Be(false);
			form.Name.Should().Be(StationName);
			form.Phone.Should().Be(StationPhone);
			form.TownId.Should().Be(StationTownId);
		}

		[Fact]
		public void Post_Add_WithAlreadyExistingStationShouldReturnView()
		{
			//Arrange
			var controller = new AdminStationsController(this.townService.Object, this.stationService.Object);

			this.stationService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
				.Returns(false);

			this.townService.Setup(t => t.GetTownNameById(It.IsAny<int>()))
				.Returns(Town);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Add(this.GetAddStationFormModel());

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<StationFormModel>();
			var form = model.As<StationFormModel>();
			form.IsEdit.Should().Be(false);
			form.Name.Should().Be(StationName);
			form.Phone.Should().Be(StationPhone);
			form.TownId.Should().Be(StationTownId);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.StationAlreadyExists, form.Name, Town));
		}

		[Fact]
		public void Post_Add_WithValidDataShouldRedirectToAllStations()
		{
			//Arrange
			var controller = new AdminStationsController(this.townService.Object, this.stationService.Object);

			this.stationService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
				.Returns(true);

			this.townService.Setup(t => t.GetTownNameById(It.IsAny<int>()))
				.Returns(Town);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Add(this.GetAddStationFormModel());

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllStations);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.StationCreated, StationName, Town));
		}

		[Fact]
		public void Get_Edit_WithInvalidStationIdShouldRedirectToAllStations()
		{
			//Arrange
			AdminStationEditServiceModel station = null;
			var controller = new AdminStationsController(null,this.stationService.Object);

			this.stationService.Setup(s => s.GetStationToEdit(It.IsAny<int>()))
				.Returns(station);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Edit(EditStationId);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllStations);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, nameof(WebConstants.Entity.Station), EditStationId));
		}

		[Fact]
		public void Get_Edit_WithValidStationIdShouldReturnView()
		{
			//Arrange
			var controller = new AdminStationsController(this.townService.Object, this.stationService.Object);

			this.stationService.Setup(s => s.GetStationToEdit(It.IsAny<int>()))
				.Returns(this.GetStationToEdit());

			//Act
			var result = controller.Edit(EditStationId);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<StationFormModel>();
			var form = model.As<StationFormModel>();
			form.Id.Should().Be(EditStationId);
			form.Name.Should().Be(StationName);
			form.Phone.Should().Be(StationPhone);
			form.TownId.Should().Be(StationTownId);
			form.IsEdit.Should().BeTrue();
		}

		[Fact]
		public void Post_Edit_WithNonExistingStationShouldRedirectToAllTowns()
		{
			//Arrange
			var controller = new AdminStationsController(null, this.stationService.Object);

			this.stationService.Setup(s => s.StationExists(It.IsAny<int>()))
				.Returns(false);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Edit(this.GetEditStationFormModel());

			//Assert
			result.Should().BeOfType<RedirectResult>();
			var model = result.As<RedirectResult>();
			model.Url.Should().Be(WebConstants.Routing.AdminAllTownsUrl);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, nameof(WebConstants.Entity.Station), EditStationId));
		}

		[Fact]
		public void Post_Edit_WhenNoChangesWereFoundShouldReturnView()
		{
			//Arrange
			var controller = new AdminStationsController(this.townService.Object, this.stationService.Object);

			this.stationService.Setup(s => s.StationExists(It.IsAny<int>()))
				.Returns(true);

			this.stationService.Setup(s => s.EditedStationIsSame(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(true);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Edit(this.GetEditStationFormModel());

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<StationFormModel>();
			var form = model.As<StationFormModel>();
			form.Id.Should().Be(EditStationId);
			form.Name.Should().Be(StationName);
			form.Phone.Should().Be(StationPhone);
			form.TownId.Should().Be(StationTownId);
			form.IsEdit.Should().BeTrue();
			this.customMessage.Should().Be(WebConstants.Message.NoChangesFound);
		}

		[Fact]
		public void Post_Edit_WithInvalidModelStateShouldReturnView()
		{
			//Arrange
			var controller = new AdminStationsController(this.townService.Object, this.stationService.Object);
			controller.ModelState.AddModelError(string.Empty, "Error");

			this.stationService.Setup(s => s.StationExists(It.IsAny<int>()))
				.Returns(true);

			this.stationService.Setup(s => s.EditedStationIsSame(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(false);

			//Act
			var result = controller.Edit(this.GetEditStationFormModel());

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<StationFormModel>();
			var form = model.As<StationFormModel>();
			form.IsEdit.Should().BeTrue();
			form.Name.Should().Be(StationName);
			form.Phone.Should().Be(StationPhone);
			form.TownId.Should().Be(StationTownId);
		}

        [Fact]
        public void Post_Edit_WhenEditedStationIsSameWithExistingOneShouldReturnView()
        {
            //Arrange
            var controller = new AdminStationsController(this.townService.Object, this.stationService.Object);

            this.stationService.Setup(s => s.StationExists(It.IsAny<int>()))
                .Returns(true);

            this.stationService.Setup(s => s.EditedStationIsSame(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(false);

            this.stationService.Setup(s => s.Edit(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(false);

            this.townService.Setup(t => t.GetTownNameById(It.IsAny<int>()))
                .Returns(TownName);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(this.GetEditStationFormModel());

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model;
            model.Should().BeOfType<StationFormModel>();
            var form = model.As<StationFormModel>();
            form.Id.Should().Be(EditStationId);
            form.Name.Should().Be(StationName);
            form.Phone.Should().Be(StationPhone);
            form.TownId.Should().Be(StationTownId);
            form.IsEdit.Should().BeTrue();
            this.customMessage.Should().Be(string.Format(WebConstants.Message.StationAlreadyExists, form.Name, TownName));
        }

        [Fact]
        public void Post_Edit_WithCorrectDataShouldRedirectToAllStations()
        {
            //Arrange
            var controller = new AdminStationsController(this.townService.Object, this.stationService.Object);

            this.stationService.Setup(s => s.StationExists(It.IsAny<int>()))
                .Returns(true);

            this.stationService.Setup(s => s.EditedStationIsSame(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(false);

            this.stationService.Setup(s => s.Edit(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);

            this.townService.Setup(t => t.GetTownNameById(It.IsAny<int>()))
                .Returns(TownName);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(this.GetEditStationFormModel());

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            this.customMessage.Should().Be(string.Format(WebConstants.Message.EntityEdited, nameof(WebConstants.Entity.Station)));
        }

        private IEnumerable<AdminStationListingServiceModel> GetStations()
		{
			var list = new List<AdminStationListingServiceModel>();

			for (int i = 0; i < 30; i++)
			{
				list.Add(new AdminStationListingServiceModel()
				{
					Id = i,
					Name = $"Station {i}"
				});
			}

			return list;
		}

		private StationFormModel GetAddStationFormModel()
		{
			return new StationFormModel()
			{
				IsEdit = false,
				Name = StationName,
				Phone = StationPhone,
				TownId = StationTownId,
				Towns = this.GenerateSelectListTowns()
			};
		}

		private StationFormModel GetEditStationFormModel()
		{
			return new StationFormModel()
			{
				Id = EditStationId,
				IsEdit = true,
				Name = StationName,
				Phone = StationPhone,
				TownId = StationTownId,
				Towns = this.GenerateSelectListTowns()
			};
		}

		private AdminStationEditServiceModel GetStationToEdit()
		{
			return new AdminStationEditServiceModel()
			{
				Id = EditStationId,
				Name = StationName,
				Phone = StationPhone,
				TownId = StationTownId
			};
		}

		private IEnumerable<SelectListItem> GenerateSelectListTowns()
		{
			var list = new List<SelectListItem>();

			for (int i = 0; i < 5; i++)
			{
				list.Add(new SelectListItem()
				{
					Text = $"Town {i}",
					Value = i.ToString()
				});
			}

			return list;
		}
	}
}
