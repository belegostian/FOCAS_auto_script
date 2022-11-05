
namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> DisconnectAsync()
        {
            return await Task.FromResult(Disconnect());
        }
        
        public dynamic Disconnect()
        {
            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas.focas_ret) Focas.cnc_freelibhndl(_handle);
            });

            var nr = new
            {
                method = "cnc_freelibhndl",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = $"{this._docBasePath}/handle/cnc_freelibhndl",
                success = ndr.RC == Focas.EW_OK,
                rc = ndr.RC,
                request = new {cnc_freelibhndl = new { }},
                response = new {cnc_freelibhndl = new { }}
            };

            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");
            
            return nr;
        }
    }
}