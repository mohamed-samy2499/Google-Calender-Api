# Google-Calender-Api
A n tier project with 'Domain Layer' 'Infrastructure' 'Application Layer' 'Api Layer'.
Repository and cqrs Design patterns were used.

Here is a Documentation for the controllers

# AuthController

The `AuthController` provides endpoints for user authentication and authorization.

## Google Login URL

### `GET /api/auth/google-login-url`

Returns the Google Login URL, allowing users to initiate the Google Sign-In process.

#### Request

- Method: `GET`

#### Response

- Status: `200 OK`
- Body: JSON object with the Google Login URL.

```json
{
  "GoogleLoginUrl": "https://example.com/auth/google-login"
}
```
## Google Login Callback 

### `GET /api/auth/callback`

Handles the callback after the user signs in using Google. Retrieves user information and generates a JWT token.

#### Request

- Method: `GET`
- Query Parameters:
  - `code` (required): The authorization code received from Google.
#### Response

- Status: `200 OK` or `400 Bad Request` 
- Body: JSON object with the JWT token or an error message.

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```
## Access Denied

### `GET /api/auth/access-denied`

Returns an unauthorized status with a message indicating that the user is unauthorized to access certain information.

#### Request

- Method: `GET`

#### Response

- Status: `401 Unauthorized` 
- Body: "This user is unauthorized to access this info."

# EventController

The `EventController` manages events, including retrieval, creation, updating, and deletion.

## Get All Google Calendar Events

### `GET /api/event/get-allGoogleCalendarEvents`

Retrieves all Google Calendar events for the authenticated user.

#### Request

- Method: `GET`
- Query Parameters:
  - `startDate` (optional): Start date for filtering events.
  - `endDate`  (optional): End date for filtering events.
  - `searchQuery` (optional): Search query for filtering events.
  - `page` (optional, default: 1): Page number for paginated results.
  - `pageSize` (optional, default: 10): Number of events per page.

#### Response

- Status: `200 OK`or `400 Bad Request`
- Body: JSON object with the list of events or an error message.

```json
{
  "events": [
    {
      "eventId": 1,
      "title": "Event 1",
      "startDateTime": "2023-01-01T12:00:00",
      "endDateTime": "2023-01-01T14:00:00"
    },
    // ... other events
  ]
}
```
## Get All Database Events

### `GET /api/event/get-allDatabaseEvents`

Retrieves all events stored in the application's database.

#### Request

- **Method:** `GET`

#### Response

- **Status:** `200 OK` or `404 Not Found`
- **Body:** JSON object with the list of events or an error message.

```json
{
  "events": [
    {
      "eventId": 1,
      "title": "Event 1",
      "startDateTime": "2023-01-01T12:00:00",
      "endDateTime": "2023-01-01T14:00:00"
    },
    // ... other events
  ]
}
```
## Get Event by Id

### `GET /api/event/get-event`

Retrieves a specific event by its identifier.

#### Request

- Method: `GET`
- Query Parameters:
  - `eventId` (required): The identifier of the event.

#### Response

- Status: `200 OK`or `404 Not Found`
- Body: JSON object with the event details or an error message.

```json
{
  "eventId": 1,
  "title": "Event 1",
  "startDateTime": "2023-01-01T12:00:00",
  "endDateTime": "2023-01-01T14:00:00"
}
```
## Add Event

### `POST /api/event/add-event`

Adds a new event to the user's Google Calendar and the application's database.

#### Request

- Method: `POST`
- Body: JSON object with event details.
```json
{
  "title": "New Event",
  "startDateTime": "2023-02-01T15:00:00",
  "endDateTime": "2023-02-01T17:00:00"
}
```
#### Response

- Status: `200 OK`or `400 Bad Request`
- Body: JSON object with the new event details or an error message.

```json
{
  "eventId": 2,
  "title": "New Event",
  "startDateTime": "2023-02-01T15:00:00",
  "endDateTime": "2023-02-01T17:00:00"
}
```

## Update Event

### `POST /api/event/update-event`

Updates an existing event in the user's Google Calendar and the application's database.

#### Request

- Method: `POST`
- Body: JSON object with updated event details.
```json
{
  "eventId": 2,
  "title": "Updated Event",
  "startDateTime": "2023-02-01T16:00:00",
  "endDateTime": "2023-02-01T18:00:00"
}
```
#### Response

- Status: `200 OK`or `400 Bad Request`
- Body: JSON object with the updated event details or an error message.

```json
{
  "eventId": 2,
  "title": "Updated Event",
  "startDateTime": "2023-02-01T16:00:00",
  "endDateTime": "2023-02-01T18:00:00"
}
```

## Delete Event

### `POST /api/event/delete-event`

Deletes an event from the user's Google Calendar and the application's database.

#### Request

- Method: `POST`
- Body: JSON object with the event identifier.
```json
{
  "eventId": 2
}
```
#### Response

- Status: `200 OK`or `400 Bad Request`
- Body: JSON object with a success message or an error message.

```json
{
  "message": "Event deleted successfully."
}
```
