using System;

namespace SC2Statistics.Utilities.Web
{
    public interface IDownloader
    {
        string GetContent(Uri url);

        byte[] GetContentAsBytes(Uri url);
    }
}