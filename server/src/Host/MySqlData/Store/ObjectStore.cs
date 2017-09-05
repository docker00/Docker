using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class ObjectStore
    {
        private readonly string _connectionString;
        private readonly ObjectRepository _objectRepository;
        private readonly ObjectClientRepository _objectClientRepository;
        private readonly ObjectEndpointRepository _objectEndpointRepository;

        public ObjectStore()
            : this("DefaultConnection")
        {

        }

        public ObjectStore(string connectionString)
        {
            _connectionString = connectionString;
            _objectRepository = new ObjectRepository(_connectionString);
            _objectClientRepository = new ObjectClientRepository(_connectionString);
            _objectEndpointRepository = new ObjectEndpointRepository(_connectionString);
        }

        public IQueryable<Object> Objects
        {
            get
            {
                return _objectRepository.GetObjects();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> CreateAsync(Object _object, CancellationToken cancellationToken)
        {
            if (_object == null)
            {
                throw new ArgumentNullException("_object");
            }
             Object oldObject = _objectRepository.GetObjectByName(_object.Name);
            if(oldObject != null)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Description = "Объект с таким именем уже существует" }));
            }
            oldObject = _objectRepository.GetObjectByUrl(_object.Url);
            if(oldObject != null)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Description = "Объект с таким Url уже существует" }));
            }
            _objectRepository.Insert(_object);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(Object _object, CancellationToken cancellationToken)
        {
            if (_object == null)
            {
                throw new ArgumentNullException("_object");
            }
            Object oldObject = _objectRepository.GetObjectByName(_object.Name);
            if (oldObject != null && !oldObject.Id.Equals(_object.Id))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Description = "Объект с таким именем уже существует" }));
            }
            oldObject = _objectRepository.GetObjectByUrl(_object.Url);
            if (oldObject != null && !oldObject.Id.Equals(_object.Id))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Description = "Объект с таким Url уже существует" }));
            }
            _objectRepository.Update(_object);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(Object _object, CancellationToken cancellationToken)
        {
            if (_object == null)
            {
                throw new ArgumentNullException("_object");
            }

            _objectEndpointRepository.DeleteByObjectId(_object.Id);
            _objectRepository.Delete(_object.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<Object> FindByClientIdAsync(string clientId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            Object _object =_objectClientRepository.GetObject(clientId);
            return Task.FromResult(_object);
        }

        public Task<Object> FindByIdAsync(string _objectId, CancellationToken cancellationToken)
        {
            Object result = _objectRepository.GetObjectById(_objectId);

            return Task.FromResult<Object>(result);
        }

        public Task<Object> FindByNameAsync(string _objectName, CancellationToken cancellationToken)
        {
            Object result = _objectRepository.GetObjectByName(_objectName);
            return Task.FromResult(result);
        }

        public Task<Object> FindByUrlAsync(string _objectUrl, CancellationToken cancellationToken)
        {
            Object result = _objectRepository.GetObjectByUrl(_objectUrl);
            return Task.FromResult(result);
        }

        public Task<int> GetObjectQueryFormatterCountAsync(string search)
        {
            return Task.FromResult(_objectRepository.GetQueryFormatterCount(search));
        }
    }
}
