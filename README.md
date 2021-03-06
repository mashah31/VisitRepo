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

<ul>
  <li>POST /api/v1/user/?FirstName={fName}&LastName={lName}&UserName={uName}</li>
  <li>User will get created with Status=Pending</li>
</ul>

<b>Post New State</b>

<ul>
  <li>POST /api/v1/state?Name={stateName}&Abbr={abbr}&Cities={cities}</li>
  <li>{stateName} State Name</li>
</ul>

<b>Post New Visit</b>

<ul>
  <li>POST /api/v1/user/mashah/visits?City=Birmingham&State=Alabama&Latitude=33.4861234&Longitude=-86.7805648</li>
  <li>Returns with success message and along with near by friends if any (within last 5 days user have pinned near by)</li>
</ul>


&nbsp;{
  &nbsp;&nbsp;"Message": "Visit have been registered!!",
  &nbsp;&nbsp;"NearByVisits": [
    &nbsp;&nbsp;&nbsp;{
      &nbsp;&nbsp;&nbsp;&nbsp;"Name": "Shaun Marsh",
      &nbsp;&nbsp;&nbsp;&nbsp;"Distance": 2.96,
      &nbsp;&nbsp;&nbsp;&nbsp;"Visited": "2015-10-06T15:05:01"
    &nbsp;&nbsp;&nbsp;}
  &nbsp;&nbsp;]
&nbsp;}

<b>Approve or Reject User</b>

<ul>
  <li>PUT /POST /api/v1/user/{username}/auth/{condition}</li>
  <li>{username}: User Name</li>
  <li>{condition}: true (approve)/ false (rejected)</li>
</ul>

<b>Get list of Users in order of Recent Visits</b>

<ul>
  <li>GET api/v1/user/?PageSize={pageSize}&PageNumber={pageNumber}</li>
  <li>{pageSize}: Number of users to return</li>
  <li>{pageNumber}: Index of page</li>
</ul>

<b>Get list of visits by user</b>

<ul>
  <li>GET api/v1/user/{username}/visits/?PageSize=2&PageNumber=0</li>
  <li>{username}: User Name</li>
  <li>Visited section: List of recent {x} visits with geo coordinates</li>
  <li>Name/Username/LastAccess section: Personal information of User</li>
  <li>Near By section: List of friends around recent visit (pinned visit). Distance is specified in miles.</li>
  <li>returns with JSON</li>
</ul>

{<br />
  &nbsp;"Name": "Shaun Marsh", "UserName": "smarsh",<br />
  &nbsp;"CreatedAt": "2012-06-19T00:00:00",<br />
  &nbsp;"Recent": {<br />
    &nbsp;&nbsp;&nbsp;"Visited": "2016-09-26T09:11:31.043",<br />
    &nbsp;&nbsp;&nbsp;"CityID": 4, "City": "Somerville", "State": "Alabama",<br />
    &nbsp;&nbsp;&nbsp;"Latitude": 34.4691003, "Longitude": -86.7928408<br />
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


<b>Get list of closest users around specified location (Geo Location)</b>

<ul>
  <li>GET /api/v1/user/{closestXUsers}?Latitude={latitude}&Longitude={longitude}</li>
  <li>{closestXUsers}: Closest X Users to Retrive</li>
  <li>{latitude}/{longitude}: GEO Location to find Users Around</li>
</ul>

<b>Get most popular states</b>

<ul>
  <li>GET /api/v1/state/{topX}</li>
  <li>{topX} Top X to retrive for popular States</li>
</ul>

<b>Get most popular cities of state</b>

<ul>
  <li>GET /api/v1/state/{state}/cities/{topX}</li>
  <li>{topX} Top X to retrive for popular cities</li>
  <li>{state} Name or Abbreviation of state are acceptable</li>
</ul>

<b>Get cities of state</b>

<ul>
  <li>GET /api/v1/state/{state}/cities/?PageNumber={pageNumber}&PageSize={pageSize}</li>
  <li>{state} Name or Abbreviation of state are acceptable</li>
</ul>

<b>Delete User</b>

<ul>
  <li>DELETE api/v1/user/{username}</li>
  <li>{username}: User Name</li>
</ul>
