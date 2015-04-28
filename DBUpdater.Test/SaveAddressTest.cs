﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using DBUpdater.Models;
using Infrastructure.AddressServices.Interfaces;
using NSubstitute;
using NUnit.Framework;
using IAddressCoordinates = Core.DomainServices.IAddressCoordinates;

namespace DBUpdater.Test
{
    [TestFixture]
    public class SaveAddressTest
    {
        private UpdateService _uut;
        private IGenericRepository<Employment> _emplRepoMock;
        private IGenericRepository<OrgUnit> _orgUnitRepoMock;
        private IGenericRepository<Person> _personRepoMock;
        private IGenericRepository<CachedAddress> _cachedAddressRepoMock;
        private IGenericRepository<PersonalAddress> _personalAddressRepoMock;
        private IAddressLaunderer _actualLaundererMock;
        private IAddressCoordinates _coordinatesMock;
        private IDBUpdaterDataProvider _dataProviderMock;

        [SetUp]
        public void SetUp()
        {
            var cachedAddressList = new List<CachedAddress>();
            var cachedIdCount = 0;
            var personalAddressList = new List<PersonalAddress>();
            var personalIdCount = 0;

            _emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            _orgUnitRepoMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            _personRepoMock = NSubstitute.Substitute.For<IGenericRepository<Person>>();
            _cachedAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<CachedAddress>>();
            _personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            _actualLaundererMock = NSubstitute.Substitute.For<IAddressLaunderer>();
            _coordinatesMock = NSubstitute.Substitute.For<IAddressCoordinates>();
            _dataProviderMock = NSubstitute.Substitute.For<IDBUpdaterDataProvider>();

            _cachedAddressRepoMock.Insert(new CachedAddress()).ReturnsForAnyArgs(x => x.Arg<CachedAddress>()).AndDoes(x => cachedAddressList.Add(x.Arg<CachedAddress>())).AndDoes(x => x.Arg<CachedAddress>().Id = cachedIdCount).AndDoes(x => cachedIdCount++);

            _cachedAddressRepoMock.AsQueryable().Returns(cachedAddressList.AsQueryable());

            _personalAddressRepoMock.Insert(new PersonalAddress()).ReturnsForAnyArgs(x => x.Arg<PersonalAddress>()).AndDoes(x => personalAddressList.Add(x.Arg<PersonalAddress>())).AndDoes(x => x.Arg<PersonalAddress>().Id = personalIdCount).AndDoes(x => personalIdCount++);

            _personalAddressRepoMock.AsQueryable().Returns(personalAddressList.AsQueryable());

            _personRepoMock.AsQueryable().Returns(new List<Person>()
            {
                new Person()
                {
                    Id = 1,
                    PersonId = 1,
                }
            }.AsQueryable());

            _uut = new UpdateService(_emplRepoMock, _orgUnitRepoMock, _personRepoMock, _cachedAddressRepoMock,
                _personalAddressRepoMock, _actualLaundererMock, _coordinatesMock, _dataProviderMock);

        }

        [Test]
        public void SaveHomeAddress_WithNonExistingPerson_ShouldThrowException()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };

            Assert.Throws<Exception>(() => _uut.SaveHomeAddress(empl, 10));
        }

        [Test]
        public void SaveHomeAddress_WithNonCachedAddress_ShouldCallActualLaunderer()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };

            _uut.SaveHomeAddress(empl,1);

            _actualLaundererMock.ReceivedWithAnyArgs().Launder(new Address());
        }

        [Test]
        public void SaveHomeAddress_WithNonCachedAddress_ShouldCallCoordinates()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };

            _uut.SaveHomeAddress(empl, 1);

            _coordinatesMock.ReceivedWithAnyArgs().GetAddressCoordinates(new Address());
        }

        [Test]
        public void SaveHomeAddress_WithNonCachedAddress_ShouldCacheAddress()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };

            _uut.SaveHomeAddress(empl, 1);

            var res =_cachedAddressRepoMock.AsQueryable();
            Assert.That(res.ElementAt(0).StreetName.Equals("Jens Baggesens Vej"));
            Assert.That(res.ElementAt(0).StreetNumber.Equals("44"));
            Assert.That(res.ElementAt(0).ZipCode.Equals(8210));
            Assert.That(res.ElementAt(0).Town.Equals("Aarhus V"));
            Assert.That(res.ElementAt(0).IsDirty.Equals(false));

        }

        [Test]
        public void SaveHomeAddress_WithCachedAddress_ShouldNotCallActualLaunderer()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };

            _cachedAddressRepoMock.Insert(new CachedAddress()
            {
                IsDirty = false,
                StreetName = "Jens Baggesens Vej",
                StreetNumber = "44",
                ZipCode = 8210,
                Town = "Aarhus V"
            });

            _uut.SaveHomeAddress(empl, 1);

            _actualLaundererMock.DidNotReceiveWithAnyArgs().Launder(new Address());

            var res = _cachedAddressRepoMock.AsQueryable();
            Assert.That(res.ElementAt(0).StreetName.Equals("Jens Baggesens Vej"));
            Assert.That(res.ElementAt(0).StreetNumber.Equals("44"));
            Assert.That(res.ElementAt(0).ZipCode.Equals(8210));
            Assert.That(res.ElementAt(0).Town.Equals("Aarhus V"));
            Assert.That(res.ElementAt(0).IsDirty.Equals(false));

        }

        [Test]
        public void SaveHomeAddress_WithCachedAddress_WithoutCoordinates_ShouldCallCoordinates()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };

            _cachedAddressRepoMock.Insert(new CachedAddress()
            {
                IsDirty = true,
                StreetName = "Jens Baggesens Vej",
                StreetNumber = "44",
                ZipCode = 8210,
                Town = "Aarhus V"
            });

            _uut.SaveHomeAddress(empl, 1);

            var res = _cachedAddressRepoMock.AsQueryable();
            Assert.That(res.ElementAt(0).StreetName.Equals("Jens Baggesens Vej"));
            Assert.That(res.ElementAt(0).StreetNumber.Equals("44"));
            Assert.That(res.ElementAt(0).ZipCode.Equals(8210));
            Assert.That(res.ElementAt(0).Town.Equals("Aarhus V"));
            Assert.That(res.ElementAt(0).IsDirty.Equals(false));

        }

        [Test]
        public void SaveHomeAddress_WithNonCachedAddress_ShouldInsertAddressInto_PersonalAddresses()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };

            _uut.SaveHomeAddress(empl, 1);

            var res = _personalAddressRepoMock.AsQueryable();
            Assert.That(res.ElementAt(0).StreetName.Equals("Jens Baggesens Vej"));
            Assert.That(res.ElementAt(0).StreetNumber.Equals("44"));
            Assert.That(res.ElementAt(0).ZipCode.Equals(8210));
            Assert.That(res.ElementAt(0).Town.Equals("Aarhus V"));
            Assert.That(res.ElementAt(0).Type.Equals(PersonalAddressType.Home));
            Assert.That(res.ElementAt(0).PersonId.Equals(1));

        }
    }
}