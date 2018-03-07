using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BirdObservationRESTService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        private const string ConnectionString =
            "Server=tcp:anbo-databaseserver.database.windows.net,1433;Initial Catalog=anbobase;Persist Security Info=False;User ID=anbo;Password=Secret12;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public List<Bird> GetBirds()
        {
            const string selectAllStudents = "select * from bird order by nameEN";
            using (SqlConnection databaseConnection = new SqlConnection(ConnectionString))
            {
                databaseConnection.Open();
                using (SqlCommand selectCommand = new SqlCommand(selectAllStudents, databaseConnection))
                {
                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        List<Bird> birdsList = new List<Bird>();
                        while (reader.Read())
                        {
                            Bird bird = ReadBird(reader);
                            birdsList.Add(bird);
                        }
                        return birdsList;
                    }
                }
            }
        }

        private static Bird ReadBird(IDataRecord reader)
        {
            int id = reader.GetInt32(0);
            string nameEnglish = reader.GetString(1);
            string nameDanish = reader.GetString(2);
            string photoUrl = reader.IsDBNull(3) ? null : reader.GetString(3);
            string userId = reader.IsDBNull(4) ? null : reader.GetString(4);
            DateTime? created; // DateTime is a struct (not a class)
            if (reader.IsDBNull(5)) created = null;
            else created = reader.GetDateTime(5);

            Bird bird = new Bird
            {
                Id = id,
                NameEnglish = nameEnglish,
                NameDanish = nameDanish,
                PhotoUrl = photoUrl,
                UserId = userId,
                Created = created
            };
            return bird;
        }

        // https://stackoverflow.com/questions/1772025/sql-data-reader-handling-null-column-values
        private static string ReadSafe(IDataRecord reader, int index)
        {
            return reader.IsDBNull(index) ? null : reader.GetString(index);
        }

        // TODO should include bird name
        public List<BirdObservationFull> GetObservations(string userid = null)
        {
            string selectAllStudents;
            bool useridIncluded = (userid != null && userid.Trim().Length > 0);
            if (!useridIncluded)
            {
                selectAllStudents =
                    "select birdObservation.*, nameEn, nameDA from birdObservation join bird on birdobservation.birdId=bird.Id order by id desc";
            }
            else
            {
                selectAllStudents =
                    "select birdObservation.*, nameEn, nameDA from birdObservation join bird on birdobservation.birdId=bird.Id where birdobservation.userid = @userid order by id desc";
            }
            using (SqlConnection databaseConnection = new SqlConnection(ConnectionString))
            {
                databaseConnection.Open();
                using (SqlCommand selectCommand = new SqlCommand(selectAllStudents, databaseConnection))
                {
                    if (useridIncluded)
                    {
                        selectCommand.Parameters.AddWithValue("@userid", userid);
                    }
                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        List<BirdObservationFull> observationsList = new List<BirdObservationFull>();
                        while (reader.Read())
                        {
                            BirdObservationFull observation = ReadBirdObservation(reader);
                            observationsList.Add(observation);
                        }
                        return observationsList;
                    }
                }
            }
        }

        public int AddObservation(BirdObservation observation)
        {
            // TODO use date from user
            const string insertStudent = "insert into birdObservation (birdId, userId, latitude, longitude, placeName, population, comment, created) values (@birdId, @userId, @latitude, @longitude, @placeName, @population, @comment, @created)";
            using (SqlConnection databaseConnection = new SqlConnection(ConnectionString))
            {
                databaseConnection.Open();
                using (SqlCommand insertCommand = new SqlCommand(insertStudent, databaseConnection))
                {
                    insertCommand.Parameters.AddWithValue("@birdId", observation.BirdId);
                    insertCommand.Parameters.AddWithValue("@userId", observation.UserId);
                    //insertCommand.Parameters.AddWithValue("@created", observation.Created);
                    DateTime? da = observation.Created;
                    if (da == null)
                    {
                        insertCommand.Parameters.AddWithValue("@created", DateTime.Now.Date);
                    }
                    else
                    {
                        insertCommand.Parameters.AddWithValue("@created", da);
                    }

                    insertCommand.Parameters.AddWithValue("@latitude", observation.Latitude);
                    insertCommand.Parameters.AddWithValue("@longitude", observation.Longitude);
                    SetParameter("placeName", observation.Placename, insertCommand);
                    insertCommand.Parameters.AddWithValue("@population", observation.Population);
                    SetParameter("comment", observation.Comment, insertCommand);
                    int rowsAffected = insertCommand.ExecuteNonQuery();
                    // TODO return new ID, or new object
                    return rowsAffected;
                }
            }
        }

        private static void SetParameter(string name, string value, SqlCommand insertCommand)
        {
            if (value == null)
            {
                insertCommand.Parameters.AddWithValue(name, DBNull.Value);
            }
            else
            {
                insertCommand.Parameters.AddWithValue(name, value);
            }
        }

        public int RemoveObservation(string id)
        {
            int idInt = int.Parse(id);
            const string deleteObservation = "delete from birdObservation where id = @id";
            using (SqlConnection databaseConnection = new SqlConnection(ConnectionString))
            {
                databaseConnection.Open();
                using (SqlCommand deleteCommand = new SqlCommand(deleteObservation, databaseConnection))
                {
                    deleteCommand.Parameters.AddWithValue("@id", idInt);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    return rowsAffected;
                }
            }
        }

        public int UpdateObservation(string id, BirdObservation observation)
        {
            return 0;
        }

        private static BirdObservationFull ReadBirdObservation(IDataRecord reader)
        {
            int id = reader.GetInt32(0);
            int birdId = reader.GetInt32(1);
            string userId = ReadSafe(reader, 2);
            DateTime? created; // DateTime is a struct (not a class). ?: does not work
            if (reader.IsDBNull(3)) { created = null; }
            else { created = reader.GetDateTime(3); }
            double latitude = reader.GetDouble(4);
            double longitude = reader.GetDouble(5);
            string placename = ReadSafe(reader, 6);
            int population = reader.GetInt32(7);
            string comment = ReadSafe(reader, 8);
            string nameEN = reader.GetString(9);
            string nameDA = reader.GetString(10);

            BirdObservationFull birdObservation = new BirdObservationFull
            {
                Id = id,
                BirdId = birdId,
                UserId = userId,
                Created = created,
                Latitude = latitude,
                Longitude = longitude,
                Placename = placename,
                Population = population,
                Comment = comment,
                NameEnglish = nameEN,
                NameDanish = nameDA

            };
            return birdObservation;
        }
    }
}