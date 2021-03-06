﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NXT.Helpers;
using NXT.Models;
using NXTWebService.Models;

namespace NXT.Services
{
    public class ServiceApi
    {
        public string Token { get; set; }
        public string Url { get; set; } = "http://nxtwebservice20170531075349.azurewebsites.net/";

        HttpClient client;

        public ServiceApi()
        {
            client = NewHttpClient();
        }

        private async Task<T> ReadAsAsync<T>(HttpContent content)
        {
            string contentString = await content.ReadAsStringAsync();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contentString);
        }
        private async Task<HttpResponseMessage> PostAsJsonAsync<T>(HttpClient client, string requestUri, T value)
        {
            string contentString = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            StringContent content = new System.Net.Http.StringContent(contentString, Encoding.UTF8, "application/json");

            return await client.PostAsync(requestUri, content);
        }

        private async Task<HttpResponseMessage> PutAsJsonAsync<T>(HttpClient client, string requestUri, T value)
        {
            string contentString = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            StringContent content = new System.Net.Http.StringContent(contentString, Encoding.UTF8, "application/json");

            return await client.PutAsync(requestUri, content);
        }

        private async Task<HttpResponseMessage> PatchAsJsonAsync<T>(HttpClient client, string requestUri, T value)
        {
            string contentString = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            StringContent content = new System.Net.Http.StringContent(contentString, Encoding.UTF8, "application/json");

            return await client.PatchAsync(requestUri, content);
        }

        public HttpClient NewHttpClient(System.Net.CookieContainer cookies = null)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true,
                AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip
            };

            if (cookies != null)
            {
                handler.UseCookies = true;
                handler.CookieContainer = cookies;
            }

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(Url)
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!String.IsNullOrEmpty(Token))
            {
                client.DefaultRequestHeaders.Add("Authorization-Token", Token);
            }

            return client;
        }


        public async Task<UserDto> GetUserBySocial(string socialID, AuthType authType)
        {
            try
            {
                var response = await client.GetAsync("api/users?socialId=" + socialID + "&authType=" + authType);
                response.EnsureSuccessStatusCode();
                return await ReadAsAsync<UserDto>(response.Content);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UserDto> GetUserByEmail(string email)
        {
            try
            {
                var response = await client.GetAsync("api/users?email=" + email);
                response.EnsureSuccessStatusCode();
                return await ReadAsAsync<UserDto>(response.Content);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UserDto> PatchDtoUser(UserDto user)
        {
            Settings.Current.User = user;

            User u = new User();
            u.AvatarUrl = user.AvatarUrl;
            u.Colour = user.Colour;
            u.Email = user.Email;
            u.ID = user.ID;
            u.UserName = user.UserName;
            switch (user.AuthType)
            {
                case AuthType.Facebook:
                    {
                        u.FacebookID = user.SocialID;
                        break;
                    }
                case AuthType.Twitter:
                    {
                        u.TwitterID = user.SocialID;
                        break;
                    }
            }

            return await PatchUser(u);
        }

        public async Task<UserDto> PatchUser(User user)
        {
            try
            {
                var response = await PatchAsJsonAsync(client, "api/users/" + user.ID, user);
                response.EnsureSuccessStatusCode();
                return await ReadAsAsync<UserDto>(response.Content);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> NewUser(User user)
        {
            try
            {
                var response = await PostAsJsonAsync(client, "api/users", user);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetFriends(Guid userId)
        {
            try
            {
                var response = await client.GetAsync("api/users/" + userId + "/friends");
                response.EnsureSuccessStatusCode();
                return await ReadAsAsync<IEnumerable<UserDto>>(response.Content);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public async Task CreateGroupCommand(GroupDto group)
        {
            try
            {
                var response = await PostAsJsonAsync(client, "api/groups", group);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task PutGroup(GroupDto group)
        {
            try
            {
                var response = await PutAsJsonAsync(client, "api/groups/" + group.ID, group);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception )
            {
                return;
            }
        }

        public async Task<List<GroupDto>> GetGroups(string userId)
        {
            try
            {
                var response = await client.GetAsync("api/groups?userid=" + userId);
                response.EnsureSuccessStatusCode();
                return await ReadAsAsync<List<GroupDto>>(response.Content);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> LeaveGroup(string groupId, string userId)
        {
            try
            {
                var response = await client.PostAsync("api/groups?leaveGroupId=" + groupId + "&userid=" + userId, new StringContent(""));
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        public async Task<List<RecordDto>> GetRecordsForGroup(string groupId)
        {
            try
            {
                var response = await client.GetAsync("api/records?groupid=" + groupId);
                response.EnsureSuccessStatusCode();
                return await ReadAsAsync<List<RecordDto>>(response.Content);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task NewRecord(RecordDto shout)
        {
            try
            {
                var response = await PostAsJsonAsync(client, "api/records", shout);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task<Byte[]> GetAvatar(string Url)
        {
            try
            {
                return await client.GetByteArrayAsync(Url);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
