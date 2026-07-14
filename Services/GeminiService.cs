using System.Net.Http.Json;
using System.Text.Json;

namespace CommunityResourceAssistant.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public string LastDiagnostic { get; private set; } = string.Empty;

        private static readonly string[] AllowedCategories =
        {
            "Food",
            "Housing",
            "Employment",
            "Healthcare",
            "Legal Aid"
        };

        public GeminiService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<GeminiMatch>> MatchCategoriesAsync(
            string situationText)
        {
            LastDiagnostic = string.Empty;

            if (string.IsNullOrWhiteSpace(situationText))
            {
                LastDiagnostic =
                    "Gemini was not called because the intake text was empty.";

                return new List<GeminiMatch>();
            }

            string? apiKey =
                _configuration["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                LastDiagnostic =
                    "Gemini API key was not found. " +
                    "Expected configuration key: Gemini:ApiKey";

                Console.WriteLine(LastDiagnostic);

                return new List<GeminiMatch>();
            }

            string prompt = $"""
                You are assisting a volunteer or staff member at a
                community-support organization.

                Review the client's situation and identify every relevant
                support category from this exact approved list:

                - Food
                - Housing
                - Employment
                - Healthcare
                - Legal Aid

                Select categories that are clearly stated or strongly implied.

                Examples of strongly implied needs:

                - Sleeping in a car, staying outdoors, couch surfing, or needing
                  a safe place to stay strongly implies Housing.
                - Not having enough to eat or skipping meals strongly implies Food.
                - Losing a job or needing help finding work strongly implies Employment.
                - Needing treatment, medication, a doctor, or mental-health care
                  strongly implies Healthcare.
                - Facing court, eviction proceedings, discrimination, or needing
                  help understanding legal rights strongly implies Legal Aid.

                For every selected category, provide one short sentence explaining
                why the volunteer may want to consider that category.

                Do not recommend specific organizations.
                Do not create categories outside the approved list.

                Client situation:
                {situationText}
                """;

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",

                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                },

                generationConfig = new
                {
                    responseMimeType = "application/json",

                    responseSchema = new
                    {
                        type = "object",

                        properties = new
                        {
                            matches = new
                            {
                                type = "array",

                                items = new
                                {
                                    type = "object",

                                    properties = new
                                    {
                                        category = new
                                        {
                                            type = "string",
                                            @enum = AllowedCategories
                                        },

                                        reason = new
                                        {
                                            type = "string"
                                        }
                                    },

                                    required = new[]
                                    {
                                        "category",
                                        "reason"
                                    }
                                }
                            }
                        },

                        required = new[]
                        {
                            "matches"
                        }
                    }
                }
            };

            try
            {
                string endpoint =
    "https://generativelanguage.googleapis.com/" +
    "v1beta/models/gemini-3.5-flash:generateContent";

                using HttpRequestMessage request =
                    new HttpRequestMessage(
                        HttpMethod.Post,
                        endpoint);

                request.Headers.Add(
                    "x-goog-api-key",
                    apiKey);

                request.Content =
                    JsonContent.Create(requestBody);

                using HttpResponseMessage response =
                    await _httpClient.SendAsync(request);

                string responseText =
                    await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    LastDiagnostic =
                        $"Gemini API error: " +
                        $"{(int)response.StatusCode} " +
                        $"{response.ReasonPhrase}. " +
                        $"Response: {responseText}";

                    Console.WriteLine(LastDiagnostic);

                    return new List<GeminiMatch>();
                }

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    LastDiagnostic =
                        "Gemini returned a successful HTTP response, " +
                        "but the response body was empty.";

                    Console.WriteLine(LastDiagnostic);

                    return new List<GeminiMatch>();
                }

                using JsonDocument apiResponse =
                    JsonDocument.Parse(responseText);

                JsonElement root =
                    apiResponse.RootElement;

                if (!root.TryGetProperty(
                        "candidates",
                        out JsonElement candidates))
                {
                    LastDiagnostic =
                        "Gemini response did not contain a candidates property. " +
                        $"Raw response: {responseText}";

                    Console.WriteLine(LastDiagnostic);

                    return new List<GeminiMatch>();
                }

                if (candidates.ValueKind != JsonValueKind.Array ||
                    candidates.GetArrayLength() == 0)
                {
                    LastDiagnostic =
                        "Gemini returned no candidates. " +
                        $"Raw response: {responseText}";

                    Console.WriteLine(LastDiagnostic);

                    return new List<GeminiMatch>();
                }

                JsonElement firstCandidate =
                    candidates[0];

                if (!firstCandidate.TryGetProperty(
                        "content",
                        out JsonElement content))
                {
                    LastDiagnostic =
                        "Gemini candidate did not contain content. " +
                        $"Raw response: {responseText}";

                    Console.WriteLine(LastDiagnostic);

                    return new List<GeminiMatch>();
                }

                if (!content.TryGetProperty(
                        "parts",
                        out JsonElement parts))
                {
                    LastDiagnostic =
                        "Gemini content did not contain parts. " +
                        $"Raw response: {responseText}";

                    Console.WriteLine(LastDiagnostic);

                    return new List<GeminiMatch>();
                }

                if (parts.ValueKind != JsonValueKind.Array ||
                    parts.GetArrayLength() == 0)
                {
                    LastDiagnostic =
                        "Gemini returned an empty parts array. " +
                        $"Raw response: {responseText}";

                    Console.WriteLine(LastDiagnostic);

                    return new List<GeminiMatch>();
                }

                if (!parts[0].TryGetProperty(
                        "text",
                        out JsonElement textElement))
                {
                    LastDiagnostic =
                        "Gemini response part did not contain text. " +
                        $"Raw response: {responseText}";

                    Console.WriteLine(LastDiagnostic);

                    return new List<GeminiMatch>();
                }

                string? generatedJson =
                    textElement.GetString();

                if (string.IsNullOrWhiteSpace(generatedJson))
                {
                    LastDiagnostic =
                        "Gemini returned an empty generated response. " +
                        $"Raw response: {responseText}";

                    Console.WriteLine(LastDiagnostic);

                    return new List<GeminiMatch>();
                }

                GeminiResult? result =
                    JsonSerializer.Deserialize<GeminiResult>(
                        generatedJson,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (result == null)
                {
                    LastDiagnostic =
                        "Gemini generated JSON, but it could not be " +
                        "deserialized into GeminiResult. " +
                        $"Generated JSON: {generatedJson}";

                    Console.WriteLine(LastDiagnostic);

                    return new List<GeminiMatch>();
                }

                if (result.Matches == null ||
                    result.Matches.Count == 0)
                {
                    LastDiagnostic =
                        "Gemini request succeeded, but Gemini returned " +
                        "no matching support categories. " +
                        $"Generated JSON: {generatedJson}";

                    Console.WriteLine(LastDiagnostic);

                    return new List<GeminiMatch>();
                }

                List<GeminiMatch> validMatches =
                    result.Matches
                        .Where(match =>
                            match != null &&
                            !string.IsNullOrWhiteSpace(
                                match.Category) &&
                            AllowedCategories.Contains(
                                match.Category.Trim(),
                                StringComparer.OrdinalIgnoreCase))
                        .Select(match => new GeminiMatch
                        {
                            Category =
                                AllowedCategories.First(category =>
                                    category.Equals(
                                        match.Category.Trim(),
                                        StringComparison.OrdinalIgnoreCase)),

                            Reason =
                                string.IsNullOrWhiteSpace(match.Reason)
                                    ? "Gemini identified this category based on the client's situation."
                                    : match.Reason.Trim()
                        })
                        .GroupBy(
                            match => match.Category,
                            StringComparer.OrdinalIgnoreCase)
                        .Select(group => group.First())
                        .ToList();

                if (validMatches.Count == 0)
                {
                    LastDiagnostic =
                        "Gemini returned categories, but none matched the " +
                        "approved category list. " +
                        $"Generated JSON: {generatedJson}";

                    Console.WriteLine(LastDiagnostic);

                    return new List<GeminiMatch>();
                }

                LastDiagnostic =
                    $"Gemini request succeeded. " +
                    $"Categories returned: " +
                    $"{string.Join(", ", validMatches.Select(match => match.Category))}";

                Console.WriteLine(LastDiagnostic);

                return validMatches;
            }
            catch (HttpRequestException exception)
            {
                LastDiagnostic =
                    $"Gemini HTTP request failed: {exception.Message}";

                Console.WriteLine(exception);

                return new List<GeminiMatch>();
            }
            catch (JsonException exception)
            {
                LastDiagnostic =
                    $"Gemini JSON parsing failed: {exception.Message}";

                Console.WriteLine(exception);

                return new List<GeminiMatch>();
            }
            catch (TaskCanceledException exception)
            {
                LastDiagnostic =
                    "Gemini request timed out or was cancelled.";

                Console.WriteLine(exception);

                return new List<GeminiMatch>();
            }
            catch (Exception exception)
            {
                LastDiagnostic =
                    $"Unexpected Gemini error: " +
                    $"{exception.GetType().Name}: " +
                    $"{exception.Message}";

                Console.WriteLine(exception);

                return new List<GeminiMatch>();
            }
        }
    }

    public class GeminiResult
    {
        public List<GeminiMatch> Matches { get; set; } = new();
    }

    public class GeminiMatch
    {
        public string Category { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;
    }
}