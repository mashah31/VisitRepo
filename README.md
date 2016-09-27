# VisitRepo

Simple Rest API for CRUD operation on User's visit data across state and city build using code first approach of Entity Framework. 

<h2>How to run project?</h2>
<ul>
  <li>Install latest version of <a href="https://visualstudiogallery.msdn.microsoft.com/27077b70-9dad-4c64-adcf-c7cf6bc9970c">NUGET Package Manager</a></li>
  <li>Go to Package Manager Console Section</li>
  <li>Run command <b><i>Install-Package EntityFramework</i></b> to install Entity Framework</li>
  <li>Run command <b><i>Enable-Migrations</i></b> to enable Migration in Project for our context</li>
  <li>For subsquent migrations based on your changes in context run <b><i>Add-Migration {tag}</i></b> for updated changes.</li>
  <li>Command <b><i>Update-Database</i></b> will apply pending migrations to database.</li>
  <li>Run project</li>
  <li>Use <a href="https://www.getpostman.com/">Postman</a> to consume Rest API.</li>
</ul>

<h2>List of basic API Usage</h2>

<b>Post New User</b>

POST /api/v1/user/?FirstName={fName}&LastName={lName}&UserName={uName}<br />
User will get created with Status=Pending<br />

<b>Approve or Reject User</b>

PUT /POST /api/v1/user/{username}/auth/{condition}<br />
{username}: User Name<br />
{condition}: true (approve)/ false (rejected)<br />

<b>Get list of Users in order of Recent Visits</b>

GET api/v1/user/?PageSize={pageSize}&PageNumber={pageNumber}<br />
{pageSize}: Number of users to return<br />
{pageNumber}: Index of page<br />

<b>Get list of visits by user</b>

GET api/v1/user/{username}/visits/?PageSize=2&PageNumber=0<br />
{username}: User Name<br /><br />
returns with JSON<br />
{<br />
  &nbsp;"Name": "Shaun Marsh", "UserName": "smarsh",<br />
  &nbsp;"CreatedAt": "2012-06-19T00:00:00",<br />
  &nbsp;"Recent": {<br />
    &nbsp;&nbsp;"Visited": "2016-09-26T09:11:31.043",<br />
    &nbsp;&nbsp;"CityID": 4, "City": "Somerville", "State": "Alabama",<br />
    &nbsp;&nbsp;"Latitude": 34.4691003, "Longitude": -86.7928408<br />
  &nbsp;},<br />
  "Visits": [<br />
    &nbsp;{<br />
      &nbsp;&nbsp;"Visited": "2016-09-26T09:11:31.043", "CityID": 4, "City": "Somerville", "State": "Alabama",<br />
      &nbsp;&nbsp;"Latitude": 34.4691003, "Longitude": -86.7928408<br />
    &nbsp;},<br />
    &nbsp;{<br />
      &nbsp;&nbsp;"Visited": "2015-10-06T15:05:01", "CityID": 3, "City": "Birmingham", "State": "Alabama",<br />
      &nbsp;&nbsp;"Latitude": 33.520295, "Longitude": -86.811504<br />
    &nbsp;}<br />
  ],<br />
  "NearBy": [ <br />
  &nbsp;{ "Name": "Kyle Barnes", "Distance": 0.4, "Visited": "2015-10-02T15:05:01" },<br />
  &nbsp;{ "Name": "Steve Bummer", "Distance": 0.42, "Visited": "2016-09-26T09:26:38.41" } ],<br />
  &nbsp;"LastUpdateTime": "2015-09-25T00:00:00"<br />
}


Visited section: List of recent {x} visits with geo coordinates<br />
Name/Username/LastAccess section: Personal information of User<br />
Near By section: List of friends around recent visit (pinned visit). Distance is specified in miles.<br />

<b>Get list of closest users</b>

GET /api/v1/user/{closestXUsers}?Latitude={latitude}&Longitude={longitude}<br />
{closestXUsers}: Closest X Users to Retrive
{latitude}/{longitude}: GEO Location to find Users Around

<b>Get most popular states</b>

GET /api/v1/state/{topX}<br />
{topX} Top X to retrive for popular States<br />

<b>Get most popular cities of state</b>

GET /api/v1/state/{state}/cities/{topX}<br />
{topX} Top X to retrive for popular cities<br />
{state} Name or Abbreviation of state are acceptable<br />

<b>Get cities of state</b>

GET /api/v1/state/{state}/cities/?PageNumber={pageNumber}&PageSize={pageSize}<br />
{state} Name or Abbreviation of state are acceptable<br />

<b>Delete User</b>

DELETE api/v1/user/{username}
{username}: User Name
