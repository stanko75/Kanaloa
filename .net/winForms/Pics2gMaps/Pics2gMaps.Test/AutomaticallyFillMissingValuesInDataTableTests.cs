namespace Pics2gMaps.Test
{
    [TestClass]
    public sealed class AutomaticallyFillMissingValuesInDataTableTests
    {
        [TestMethod]
        public void AutomaticallyFillMissingValuesInDataTableCheckIfOk()
        {
            AutomaticallyFillMissingValuesInDataTableCommand automaticallyFillMissingValuesCommand =
                new AutomaticallyFillMissingValuesInDataTableCommand
                {
                    DataRow = dataRow,
                    Columns = _dtGalleryConfiguration.Columns,
                    BaseUrl = BaseUrl,
                    JqueryVersion = JqueryVersion
                };
        }
    }
}