using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Persistence;
using MyPics.Infrastructure.Repositories;
using NUnit.Framework;

namespace MyPics.Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class AuthRepositoryTests
    {
        private MyPicsDbContext _context;
        private AuthRepository _repository;

        [SetUp]
        public void Setup()
        {
            var mockKeySection = new Mock<IConfigurationSection>();
            mockKeySection.Setup(c => c.Value)
                .Returns("TAf30yv4g15177S6EW6idxfE5YxyJiCX8Wf2c4nf9Aw=");
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c.GetSection(It.IsAny<string>()))
                .Returns(() => mockKeySection.Object);
            
            var options = new DbContextOptionsBuilder<MyPicsDbContext>()
                .UseInMemoryDatabase(databaseName: "my_pics")
                .Options;

            _context = new MyPicsDbContext(options, configuration.Object);

            CreatePasswordHash("testPassword1", out var passwordHash, out var passwordSalt);
            _context.Users.Add(new User
            {
                Username = "testUsername100",
                Email = "test1@email.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RegistrationToken = "testToken1",
                RegistrationTokenGeneratedTime = DateTime.UtcNow
            });
            CreatePasswordHash("testPassword2", out passwordHash, out passwordSalt);
            _context.Users.Add(new User
            {
                Username = "testUsername200",
                Email = "test2@email.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RegistrationToken = "testToken2",
                RegistrationTokenGeneratedTime = DateTime.UtcNow
            });
            CreatePasswordHash("testPassword3", out passwordHash, out passwordSalt);
            _context.Users.Add(new User
            {
                Email = "test2@email.com",
                Username = "testUsername300",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RegistrationToken = "testToken3",
                RegistrationTokenGeneratedTime = DateTime.UtcNow.AddHours(-3).AddSeconds(-1)
            });
            _context.SaveChanges();

            _repository = new AuthRepository(_context);
        }
        
        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
        }

        [TestCase("testUsername1", "testPassword1", "test1@email.com")]
        [TestCase("testUsername2", "testPassword2", "test2@email.com")]
        [TestCase("testUsername3", "testPassword3", "test3@email.com")]
        public async Task Register_CorrectParameters_ReturnsExpectedUser(string username, string password, string email)
        {
            var user = new User
            {
                Username = username,
                Email = email
            };

            var result = await _repository.Register(user, password);
            var passwordIsMatching = VerifyPasswordHash(password, result.PasswordHash, result.PasswordSalt);
            
            result.Should().NotBeNull();
            result.Username.Should().Be(username);
            result.Email.Should().Be(email);
            result.PasswordHash.Should().NotBeEmpty();
            result.PasswordSalt.Should().NotBeEmpty();
            passwordIsMatching.Should().BeTrue();
        }

        [Test]
        public async Task Register_NullUser_ReturnsNull()
        {
            var result = await _repository.Register(null, "password");

            result.Should().BeNull();
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Register_PasswordNullOrEmpty_ReturnsNull(string password)
        {
            var user = new User
            {
                Username = "testUsername",
                Email = "test@email.com"
            };

            var result = await _repository.Register(user, password);

            result.Should().BeNull();
        }

        [Test]
        public async Task Register_Exception_ReturnsNull()
        {
            _repository = new AuthRepository(null);
            
            var user = new User
            {
                Username = "testUsername",
                Email = "test@email.com"
            };

            var result = await _repository.Register(user, "password");

            result.Should().BeNull();
        }

        [TestCase("testUsername100", "testPassword1")]
        [TestCase("testUsername200", "testPassword2")]
        [TestCase("testUsername300", "testPassword3")]
        public async Task Login_CorrectCredentials_ReturnsExpectedUser(string username, string password)
        {
            var result = await _repository.Login(username, password);
            var passwordIsMatching = VerifyPasswordHash(password, result.PasswordHash, result.PasswordSalt);
            
            result.Should().NotBeNull();
            result.Username.Should().Be(username);
            passwordIsMatching.Should().BeTrue();
        }

        [TestCase("notExistingTestUsername1", "notExistingTestPassword1")]
        [TestCase("notExistingTestUsername2", "notExistingTestPassword2")]
        [TestCase(null, null)]
        public async Task Login_NotExistingOrNullCredentials_ReturnsNull(string username, string password)
        {
            var result = await _repository.Login(username, password);

            result.Should().BeNull();
        }

        [Test]
        public async Task Login_Exception_ReturnsNull()
        {
            _repository = new AuthRepository(null);

            var result = await _repository.Login("testUsername100", "testPassword1");

            result.Should().BeNull();
        }

        [TestCase("testUsername100")]
        [TestCase("testUsername200")]
        [TestCase("testUsername300")]
        public async Task UserExists_ExistingUsername_ReturnsTrue(string username)
        {
            var result = await _repository.UserExists(username);

            result.Should().BeTrue();
        }
        
        [TestCase("notExistingUsername1")]
        [TestCase("notExistingUsername2")]
        [TestCase("notExistingUsername3")]
        public async Task UserExists_NotExistingUsername_ReturnsFalse(string username)
        {
            var result = await _repository.UserExists(username);

            result.Should().BeFalse();
        }

        [Test]
        public async Task UserExist_Exception_ReturnsTrue()
        {
            _repository = new AuthRepository(null);

            var result = await _repository.UserExists("testUsername100");

            result.Should().BeTrue();
        }
        
        [TestCase("test1@email.com")]
        [TestCase("test2@email.com")]
        [TestCase("test2@email.com")]
        public async Task EmailExists_ExistingEmail_ReturnsTrue(string email)
        {
            var result = await _repository.EmailExists(email);

            result.Should().BeTrue();
        }
        
        [TestCase("notExistingtest1@email.com")]
        [TestCase("notExistingtest2@email.com")]
        [TestCase("notExistingtest3@email.com")]
        public async Task EmailExists_NotExistingEmail_ReturnsFalse(string email)
        {
            var result = await _repository.EmailExists(email);

            result.Should().BeFalse();
        }

        [Test]
        public async Task EmailExist_Exception_ReturnsTrue()
        {
            _repository = new AuthRepository(null);

            var result = await _repository.EmailExists("test1@email.com");

            result.Should().BeTrue();
        }

        [TestCase("testToken1", "testUsername100")]
        [TestCase("testToken2", "testUsername200")]
        public async Task ConfirmEmail_Successful_ReturnsTrue(string token, string username)
        {
            var result = await _repository.ConfirmEmail(token, username);

            result.Should().BeTrue();
        }
        
        [TestCase("testToken1", "testUsername50")]
        [TestCase("testToken2", "testUsername60")]
        public async Task ConfirmEmail_UnSuccessful_NullUser_ReturnsFalse(string token, string username)
        {
            var result = await _repository.ConfirmEmail(token, username);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task ConfirmEmail_UnSuccessful_Expired_ReturnsFalse()
        {
            var result = await _repository.ConfirmEmail("testToken3", "testUsername300");

            result.Should().BeFalse();
        }
        
        [TestCase("notExistingToken1", "testUsername50")]
        [TestCase("notExistingToken2", "testUsername60")]
        public async Task ConfirmEmail_UnSuccessful_WrongToken_ReturnsFalse(string token, string username)
        {
            var result = await _repository.ConfirmEmail(token, username);

            result.Should().BeFalse();
        }

        [Test]
        public async Task ConfirmEmail_Exception_ReturnsFalse()
        {
            _repository = new AuthRepository(null);
            
            var result = await _repository.ConfirmEmail("testToken1", "testUsername100");

            result.Should().BeFalse();
        }
        
        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return !computedHash.Where((t, i) => t != passwordHash[i]).Any();
        }
        
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}