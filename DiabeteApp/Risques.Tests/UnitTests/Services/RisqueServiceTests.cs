using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Risques.DTOs;
using Risques.Services.Implementations;
using Risques.Services.Interfaces;

namespace Risques.Tests.UnitTests.Services
{
    public class RisqueServiceTests
    {
        private readonly Mock<IPatientApiService> _patientApiServiceMock;
        private readonly Mock<INoteApiService> _noteApiServiceMock;
        private readonly Mock<IDateProvider> _dateProviderMock;
        private readonly RisqueService _service;

        public RisqueServiceTests()
        {
            _patientApiServiceMock = new Mock<IPatientApiService>();
            _noteApiServiceMock = new Mock<INoteApiService>();
            _dateProviderMock = new Mock<IDateProvider>();
            _service = new RisqueService(_patientApiServiceMock.Object, _noteApiServiceMock.Object, _dateProviderMock.Object);
        }

        // -- CalculerAgePatientAsync --
        [Fact]
        public async Task CalculerAgePatientAsync_DateDebutAnnee_Retourne30()
        {
            // Arrange
            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 6, 1));

            // Act
            var result = await _service.CalculerAgePatientAsync(new DateTime(2000, 1, 1));

            // Assert
            Assert.Equal(30, result);
        }

        [Fact]
        public async Task CalculerAgePatientAsync_DateFinAnnee_Retourne29()
        {
            // Arrange
            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 6, 1));

            // Act
            var result = await _service.CalculerAgePatientAsync(new DateTime(2000, 12, 31));

            // Assert
            Assert.Equal(29, result);
        }


        // -- CalculerNombreDeclencheursAsync --
        [Fact]
        public async Task CalculerNombreDeclencheursAsync_0Dceclencheur_Retourne0()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Test avec 0 déclencheur." }
            };

            // Act
            var result = await _service.CalculerNombreDeclencheursAsync(notes);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task CalculerNombreDeclencheursAsync_3Dceclencheurs_Retourne3()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Cholestérol détecté." },
                new NoteDto { Contenu = "Le patient présente une réaction anormale." }
            };

            // Act
            var result = await _service.CalculerNombreDeclencheursAsync(notes);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task CalculerNombreDeclencheursAsync_ContenuVide_Retourne0()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto
                {
                    Contenu = ""
                }
            };

            // Act
            var result = await _service.CalculerNombreDeclencheursAsync(notes);

            // Assert
            Assert.Equal(0, result);
        }


        // -- EvaluerRisqueAsync --
        // 0 déclencheur
        [Fact]
        public async Task EvaluerRisqueAsync_AucunDeclencheur_RetourneAucunRisque()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Test avec 0 déclencheur." }
            };

            _patientApiServiceMock.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(new PatientDto());

            _noteApiServiceMock.Setup(s => s.GetNotesByPatientAsync(1))
                .ReturnsAsync(new List<NoteDto>(notes));

            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 06, 01));

            // Act
            var result = await _service.EvaluerRisqueAsync(1);

            // Assert
            var dto = Assert.IsType<RisqueDto>(result);
            Assert.Equal("Aucun risque", dto.Risque);
        }

        // Âge >= 30 ans
        [Fact]
        public async Task EvaluerRisqueAsync_AgePlus30Et1Declencheur_RetourneAucunRisque()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Test avec 1 déclencheur : cholestérol." }
            };

            _patientApiServiceMock.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(new PatientDto { DateDeNaissance = new DateTime(2000, 1, 1) });

            _noteApiServiceMock.Setup(s => s.GetNotesByPatientAsync(1))
                .ReturnsAsync(new List<NoteDto>(notes));

            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 06, 01));

            // Act
            var result = await _service.EvaluerRisqueAsync(1);

            // Assert
            var dto = Assert.IsType<RisqueDto>(result);
            Assert.Equal("Aucun risque", dto.Risque);
        }

        [Fact]
        public async Task EvaluerRisqueAsync_AgePlus30Et2Declencheurs_RetourneRisqueLimite()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Test avec 2 déclencheurs : cholestérol et anticorps." }
            };

            _patientApiServiceMock.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(new PatientDto { DateDeNaissance = new DateTime(2000, 1, 1) });

            _noteApiServiceMock.Setup(s => s.GetNotesByPatientAsync(1))
                .ReturnsAsync(new List<NoteDto>(notes));

            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 06, 01));

            // Act
            var result = await _service.EvaluerRisqueAsync(1);

            // Assert
            var dto = Assert.IsType<RisqueDto>(result);
            Assert.Equal("Risque limité", dto.Risque);
        }

        [Fact]
        public async Task EvaluerRisqueAsync_AgePlus30Et6Declencheurs_RetourneDanger()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Test avec 6 déclencheurs : cholestérol, microalbumine, taille, poids, fumeur et anticorps." }
            };

            _patientApiServiceMock.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(new PatientDto { DateDeNaissance = new DateTime(2000, 1, 1) });

            _noteApiServiceMock.Setup(s => s.GetNotesByPatientAsync(1))
                .ReturnsAsync(new List<NoteDto>(notes));

            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 06, 01));

            // Act
            var result = await _service.EvaluerRisqueAsync(1);

            // Assert
            var dto = Assert.IsType<RisqueDto>(result);
            Assert.Equal("Danger", dto.Risque);
        }

        [Fact]
        public async Task EvaluerRisqueAsync_AgePlus30Et8Declencheurs_RetourneApparitionPrecoce()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Test avec 8 déclencheurs : cholestérol, microalbumine, taille, poids, fumeur, anormal, vertiges et anticorps." }
            };

            _patientApiServiceMock.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(new PatientDto { DateDeNaissance = new DateTime(2000, 1, 1) });

            _noteApiServiceMock.Setup(s => s.GetNotesByPatientAsync(1))
                .ReturnsAsync(new List<NoteDto>(notes));

            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 06, 01));

            // Act
            var result = await _service.EvaluerRisqueAsync(1);

            // Assert
            var dto = Assert.IsType<RisqueDto>(result);
            Assert.Equal("Apparition précoce", dto.Risque);
        }

        // Âge < 30 ans et genre = M
        [Fact]
        public async Task EvaluerRisqueAsync_AgeMoins30GenreMEt1Declencheur_RetourneAucunRisque()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Test avec 1 déclencheur : cholestérol." }
            };

            _patientApiServiceMock.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(new PatientDto { Genre = "M", DateDeNaissance = new DateTime(2000, 12, 31) });

            _noteApiServiceMock.Setup(s => s.GetNotesByPatientAsync(1))
                .ReturnsAsync(new List<NoteDto>(notes));

            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 06, 01));

            // Act
            var result = await _service.EvaluerRisqueAsync(1);

            // Assert
            var dto = Assert.IsType<RisqueDto>(result);
            Assert.Equal("Aucun risque", dto.Risque);
        }

        [Fact]
        public async Task EvaluerRisqueAsync_AgeMoins30GenreMEt3Declencheurs_RetourneDanger()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Test avec 3 déclencheurs : cholestérol, microalbumine et anticorps." }
            };

            _patientApiServiceMock.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(new PatientDto { Genre = "M", DateDeNaissance = new DateTime(2000, 12, 31) });

            _noteApiServiceMock.Setup(s => s.GetNotesByPatientAsync(1))
                .ReturnsAsync(new List<NoteDto>(notes));

            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 06, 01));

            // Act
            var result = await _service.EvaluerRisqueAsync(1);

            // Assert
            var dto = Assert.IsType<RisqueDto>(result);
            Assert.Equal("Danger", dto.Risque);
        }

        [Fact]
        public async Task EvaluerRisqueAsync_AgeMoins30GenreMEt5Declencheurs_RetourneApparitionPrecoce()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Test avec 5 déclencheurs : cholestérol, microalbumine, taille, poids et anticorps." }
            };

            _patientApiServiceMock.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(new PatientDto { Genre = "M", DateDeNaissance = new DateTime(2000, 12, 31) });

            _noteApiServiceMock.Setup(s => s.GetNotesByPatientAsync(1))
                .ReturnsAsync(new List<NoteDto>(notes));

            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 06, 01));

            // Act
            var result = await _service.EvaluerRisqueAsync(1);

            // Assert
            var dto = Assert.IsType<RisqueDto>(result);
            Assert.Equal("Apparition précoce", dto.Risque);
        }

        // Âge < 30 ans et genre = F
        [Fact]
        public async Task EvaluerRisqueAsync_AgeMoins30GenreFEt1Declencheur_RetourneAucunRisque()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Test avec 1 déclencheur : cholestérol." }
            };

            _patientApiServiceMock.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(new PatientDto { Genre = "F", DateDeNaissance = new DateTime(2000, 12, 31) });

            _noteApiServiceMock.Setup(s => s.GetNotesByPatientAsync(1))
                .ReturnsAsync(new List<NoteDto>(notes));

            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 06, 01));

            // Act
            var result = await _service.EvaluerRisqueAsync(1);

            // Assert
            var dto = Assert.IsType<RisqueDto>(result);
            Assert.Equal("Aucun risque", dto.Risque);
        }

        [Fact]
        public async Task EvaluerRisqueAsync_AgeMoins30GenreFEt4Declencheurs_RetourneDanger()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Test avec 4 déclencheurs : cholestérol, microalbumine, taille et anticorps." }
            };

            _patientApiServiceMock.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(new PatientDto { Genre = "F", DateDeNaissance = new DateTime(2000, 12, 31) });

            _noteApiServiceMock.Setup(s => s.GetNotesByPatientAsync(1))
                .ReturnsAsync(new List<NoteDto>(notes));

            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 06, 01));

            // Act
            var result = await _service.EvaluerRisqueAsync(1);

            // Assert
            var dto = Assert.IsType<RisqueDto>(result);
            Assert.Equal("Danger", dto.Risque);
        }

        [Fact]
        public async Task EvaluerRisqueAsync_AgeMoins30GenreFEt7Declencheurs_RetourneApparitionPrecoce()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Test avec 7 déclencheurs : cholestérol, microalbumine, taille, poids, fumeuse, vertiges et anticorps." }
            };

            _patientApiServiceMock.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(new PatientDto { Genre = "F", DateDeNaissance = new DateTime(2000, 12, 31) });

            _noteApiServiceMock.Setup(s => s.GetNotesByPatientAsync(1))
                .ReturnsAsync(new List<NoteDto>(notes));

            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 06, 01));

            // Act
            var result = await _service.EvaluerRisqueAsync(1);

            // Assert
            var dto = Assert.IsType<RisqueDto>(result);
            Assert.Equal("Apparition précoce", dto.Risque);
        }


        // Âge < 30 ans et sans genre
        [Fact]
        public async Task EvaluerRisqueAsync_AgeMoins30GenreInconnuEt2Declencheurs_RetourneImpossible()
        {
            // Arrange
            var notes = new List<NoteDto>
            {
                new NoteDto { Contenu = "Test avec 2 déclencheurs : cholestérol et anticorps." }
            };

            _patientApiServiceMock.Setup(s => s.GetPatientByIdAsync(1))
                .ReturnsAsync(new PatientDto { DateDeNaissance = new DateTime(2000, 12, 31) });

            _noteApiServiceMock.Setup(s => s.GetNotesByPatientAsync(1))
                .ReturnsAsync(new List<NoteDto>(notes));

            _dateProviderMock.Setup(d => d.Today)
                .Returns(new DateTime(2030, 06, 01));

            // Act
            var result = await _service.EvaluerRisqueAsync(1);

            // Assert
            var dto = Assert.IsType<RisqueDto>(result);
            Assert.Equal("Impossible de déterminer le risque", dto.Risque);
        }
    }
};
