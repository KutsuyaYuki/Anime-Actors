using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AnimeActors.Models;
using GraphQL.Client.Http;
using Newtonsoft.Json.Linq;
using System.Linq;
using RestSharp;
using RestSharp.Serialization.Json;
using Xamarin.Essentials;
using GraphQL;

namespace AnimeActors.Services
{
    class AnilistService
    {
        //private RestClient _client = new RestClient();
        private GraphQLHttpClient _graphqlClient = new GraphQLHttpClient($"https://graphql.anilist.co");

        public AnilistService()
        {
            //_client.BaseUrl = new Uri("https://anilist.co/api/v2");
            //Connection().ConfigureAwait(false);
        }

        //private async Task Connection()
        //{
        //    RestRequest request = new RestRequest("oauth/token", Method.POST);
        //    request.AddParameter("grant_type", "authorization_code");
        //    request.AddParameter("client_id", "2807");
        //    request.AddParameter("client_secret", "rBO4u1ifoShDqaLjcNlCHh0hGWBtjC3siOKBgD5Q");
        //    request.AddParameter("redirect_uri", "https://anilist.co/api/v2/oauth/pin");
        //    request.AddParameter("code",
        //        "def502004d29e94353ba4bb8fd0ddd8e10b2618e5e133904bf62c2cfb6f3582b94b65af5fc0944a99ec4ebf7830a3472ffd740fb8084165e13cc0f622865b6b416074d3fa05b2194593bae342889fcd59ec2908e5e1a74765204f4746630252c97f5e92de219893923e786ee827d617acf79ffa61e3385861625cbfb9418a733842c0bc8eb52ae6a04763d8176210bce930ff6df8a54b1841c733c857f25f589c6f729932902f9389568a4e4b408cc74fde722dee8334d627a69b737f05964b697a4243726b0fa047803ee968054a0fa9de5ba21508862fec4bf9f52c92fe7009c71bb0f763dea7cd7cfb42b7db3acd7d5268ab6e61f0b62a031d2739a049dd4090fad4a7ebdaeb063850ccd20526a3cd7f8b35a7121d429ce7b29b38073c07cedeb916be45fc356c90037156e6724e681d13d0f11a7ca82641b18c1b1245a3f2e3cfac118341c3d41b5fb55dfa671777e66337fdf676913a4e73595c4ac2ac077b84c11b801bf02e41f2a00691bede0");
        //    var response = await _client.PostAsync<dynamic>(request);

        //    try
        //    {
        //        await SecureStorage.SetAsync("oauth_token", response);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Possible that device doesn't support secure storage on device.
        //    }
        //}

        public async Task<List<Edge>> GetVoiceActorByCharacter(string character)
        {
            var graphQLHttpRequest = new GraphQLRequest
            {
                Query = @"query Characters($page: Int, $character: String, $characterPage: Int = 1) {
                          Page(page: $page, perPage: 50) {
                            pageInfo {
                              total
                              currentPage
                              lastPage
                              hasNextPage
                              perPage
                            }
                            characters(search: $character) {
                              name {
                                first
                                last
                                full
                                native
                              }
                              image {
                                large
                              }
                              media(type: ANIME) {
                                edges {
                                  node {
                                    type
                                    title {
                                      userPreferred
                                    }
                                  }
                                  id
                                  characterRole
                                  voiceActors(language: JAPANESE) {
                                    name {
                                      full
                                    }
                                    image {
                                      large
                                    }
                                    language
                                    characters(page: $characterPage, perPage: 25) {
                                      pageInfo {
                                        total
                                        currentPage
                                        lastPage
                                        hasNextPage
                                        perPage
                                      }
                                      nodes {
                                        id
                                        name {
                                          full
                                        }
                                        image {
                                          large
                                        }
                                        media {
                                          edges {
                                            node {
                                              type
                                              title {
                                                userPreferred
                                              }
                                            }
                                          }
                                        }
                                      }
                                    }
                                  }
                                }
                              }
                            }
                          }
                        }

                    ", OperationName = "Characters"
            };
            bool hasNextPage = true;

            List<Page> voiceActorsResult = new List<Page>();

            int pagenum = 1;

            while (hasNextPage)
            {
                graphQLHttpRequest.Variables = new { page = pagenum, character = character };
                var graphQLHttpResponse = await _graphqlClient.SendQueryAsync<Data>(graphQLHttpRequest);

                var result = graphQLHttpResponse.Data;

                voiceActorsResult.Add(result.page);
                hasNextPage = result.page.pageInfo.hasNextPage;


                bool hasCharacterPage = true;

                int characterPagenum = 1;

                while (hasCharacterPage)
                {
                    graphQLHttpRequest.Variables = new { page = pagenum, character = character, characterPage = characterPagenum };
                    var characterGraphQLHttpResponse = await _graphqlClient.SendQueryAsync<Data>(graphQLHttpRequest);

                    var characterResult = characterGraphQLHttpResponse.Data;

                    voiceActorsResult.Add(characterResult.page);
                    hasCharacterPage = characterResult.page.characters.Any(
                        chara => chara.media.edges.Any(
                        page => page.voiceActors.Any(
                            va => va.characters.pageInfo.hasNextPage)
                        ));

                    characterPagenum++;
                }
                pagenum++;
            }

            var a = voiceActorsResult.SelectMany(voiceActor => voiceActor.characters.SelectMany(chara => chara.media.edges)).ToList();
            return a;
        }
    }
}