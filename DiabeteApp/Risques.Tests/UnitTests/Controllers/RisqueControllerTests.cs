using Microsoft.AspNetCore.Mvc;
using Moq;
using Risques.DTOs;
using Risques.Services.Interfaces;

namespace Risques.Tests.UnitTests.Controllers
{
    public class RisqueControllerTests
    {
        private readonly Mock<IRisqueService> _serviceMock;
        private readonly Mock<IPatientApiService> _patientApiService;
        private readonly RisqueController _controller;

        public RisqueControllerTests()
        {
            _serviceMock = new Mock<IRisqueService>();
            _patientApiService = new Mock<IPatientApiService>();
            _controller = new RisqueController(_serviceMock.Object, _patientApiService.Object);
        }


        // -- GetRisqueByPatient --

        [Fact]
        public async Task GetRisqueByPatient_None_RetourneOkAvecReponseAucunRisque()
        {
            // Arrange
            var patient = new PatientDto
            {
                Id = 1,
                Prenom = "Prénom Test 1",
                Nom = "Nom Test 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            var risque = new RisqueDto
            {
                IdPatient = 1,
                Risque = "Aucun risque"
            };

            _patientApiService.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(patient);

            _serviceMock.Setup(s => s.EvaluerRisqueAsync(1))
                .ReturnsAsync(risque);

            // Act
            var result = await _controller.GetRisqueByPatient(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<RisqueDto>(ok.Value);
            Assert.Equal("Aucun risque", dto.Risque);
        }

        [Fact]
        public async Task GetRisqueByPatient_Borderline_RetourneOkAvecReponseRisqueLimite()
        {
            // Arrange
            var patient = new PatientDto
            {
                Id = 1,
                Prenom = "Prénom Test 1",
                Nom = "Nom Test 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            var risque = new RisqueDto
            {
                IdPatient = 1,
                Risque = "Risque limité"
            };

            _patientApiService.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(patient);

            _serviceMock.Setup(s => s.EvaluerRisqueAsync(1))
                .ReturnsAsync(risque);

            // Act
            var result = await _controller.GetRisqueByPatient(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<RisqueDto>(ok.Value);
            Assert.Equal("Risque limité", dto.Risque);
        }

        [Fact]
        public async Task GetRisqueByPatient_InDanger_RetourneOkAvecReponseDanger()
        {
            // Arrange
            var patient = new PatientDto
            {
                Id = 1,
                Prenom = "Prénom Test 1",
                Nom = "Nom Test 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            var risque = new RisqueDto
            {
                IdPatient = 1,
                Risque = "Danger"
            };

            _patientApiService.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(patient);

            _serviceMock.Setup(s => s.EvaluerRisqueAsync(1))
                .ReturnsAsync(risque);

            // Act
            var result = await _controller.GetRisqueByPatient(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<RisqueDto>(ok.Value);
            Assert.Equal("Danger", dto.Risque);
        }

        [Fact]
        public async Task GetRisqueByPatient_EarlyOnset_RetourneOkAvecReponseApparitionPrecoce()
        {
            // Arrange
            var patient = new PatientDto
            {
                Id = 1,
                Prenom = "Prénom Test 1",
                Nom = "Nom Test 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            var risque = new RisqueDto
            {
                IdPatient = 1,
                Risque = "Apparition précoce"
            };

            _patientApiService.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(patient);

            _serviceMock.Setup(s => s.EvaluerRisqueAsync(1))
                .ReturnsAsync(risque);

            // Act
            var result = await _controller.GetRisqueByPatient(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<RisqueDto>(ok.Value);
            Assert.Equal("Apparition précoce", dto.Risque);
        }

        [Fact]
        public async Task GetRisqueByPatient_IdPatientInexistant_RetourneNotFound()
        {
            // Arrange
            var idPatient = 999;

            _patientApiService.Setup(s => s.GetPatientByIdAsync(idPatient))
                .ReturnsAsync((PatientDto)null);

            // Act
            var result = await _controller.GetRisqueByPatient(idPatient);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Le patient avec l'ID {idPatient} n'a pas été trouvé.", notFound.Value);
        }

        [Fact]
        public async Task GetRisqueByPatient_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange
            var patient = new PatientDto
            {
                Id = 1,
                Prenom = "Prénom Test 1",
                Nom = "Nom Test 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            _patientApiService.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(patient);

            _serviceMock.Setup(s => s.EvaluerRisqueAsync(1))
                .ThrowsAsync(new Exception("Erreur de test"));

            // Act
            var result = await _controller.GetRisqueByPatient(1);
            
            // Assert
            var internalServerError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerError.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerError.Value);
        }
    }
}
