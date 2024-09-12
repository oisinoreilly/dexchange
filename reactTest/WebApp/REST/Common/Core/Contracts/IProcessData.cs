using Models;

namespace Core.Contracts
{
    public interface IProcessData
    {
        bool CreateProcessData(string dbName, string nodeName, string itemName);
        ProcessData ReadProcessData(string dbName, string nodeName, string itemName);
        bool UpdateProcessData(string dbName, string nodeName, string itemName);
        bool DeleteProcessData(string dbName, string nodeName, string itemName);

        //ProcessData GetProcessData(string dbName, string nodeName);
        //IList<string> GetProcessDatas(string dbName, string nodeName);
    }
}
