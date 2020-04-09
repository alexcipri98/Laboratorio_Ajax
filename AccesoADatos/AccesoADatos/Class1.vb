Imports System.Data
Imports System.Data.SqlClient

Public Class Class1
    Private Shared conexion As New SqlConnection
    Private Shared comando As New SqlCommand

    'Conectarse a la BD'
    Public Shared Function conectar() As String
        Try
            conexion.ConnectionString = “Server=tcp:hads.database.windows.net,1433;Initial Catalog=Amigos;Persist Security Info=False;User ID=vadillo@ehu.es@hads;Password=curso19-20;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
            conexion.Open()
        Catch ex As Exception
            Return "ERROR DE CONEXIÓN: " + ex.Message
        End Try
        Return "CONEXION OK"
    End Function

    'Desconectarse de la BD'
    Public Shared Sub cerrarconexion()
        conexion.Close()
    End Sub



    'INSERCIONES (Hasta la linea 56)' 

    'Inserta una nueva tarea instanciada'
    Public Shared Function instanciarT(ByVal em As String, ByVal cod As String, ByVal Hest As Integer, ByVal Hrea As Integer) As String
        Dim st = "insert into EstudiantesTareas (Email, CodTarea,HEstimadas,HReales) values ('" & em & "','" & cod & "'," & Hest & "," & Hrea & ")"
        Dim numregs As Integer
        comando = New SqlCommand(st, conexion)
        Try
            numregs = comando.ExecuteNonQuery()
        Catch ex As Exception
            Return ex.Message
        End Try
        Return (numregs & " registro(s) insertado(s) en la BD ")
    End Function


    'Inserta un nuevo Usuario a la BD. entrada --> email, nombre, apellido(s), numeroDeConfirmación, tipo, password'
    Public Shared Function insertar(ByVal em As String, ByVal nom As String, ByVal ape As String, ByVal num As Integer, ByVal tip As String, ByVal pass As String) As String
        If compruebaEmail(em) = "True" Then
            Return "fail"
        Else
            Dim st = "insert into usuarios (email,nombre,apellidos,numconfir,confirmado,tipo,pass,codpass) values ('" & em & "','" & nom & "','" & ape & "'," & num & "," & 0 & ",'" & tip & "','" & pass & "'," & 0 & ")"
            Dim numregs As Integer
            comando = New SqlCommand(st, conexion)
            Try
                numregs = comando.ExecuteNonQuery()
            Catch ex As Exception
                Return ex.Message
            End Try
            Return (numregs & " registro(s) insertado(s) en la BD ")
        End If
    End Function

    'Añade una nueva tarea'
    Public Shared Function añadeTraea(ByVal codigo As String, ByVal descripcion As String, ByVal Asignatura As String, ByVal HorasEstimadas As Integer, ByVal TipoTarea As String) As String
        Dim st = "insert into TareasGenericas (Codigo,Descripcion,CodAsig,HEstimadas,Explotacion,TipoTarea) values ('" & codigo & "','" & descripcion & "','" & Asignatura & "'," & HorasEstimadas & "," & 0 & ",'" & TipoTarea & "')"
        Dim numregs As Integer
        comando = New SqlCommand(st, conexion)
        Try
            numregs = comando.ExecuteNonQuery()
        Catch ex As Exception
            Return ex.Message
        End Try
        Return (numregs & " registro(s) insertado(s) en la BD ")
    End Function

    'CONSULTAS (hasta la linea 145)'


    'Devuelve las asignaturas de un estudiante'
    Public Shared Function visualizarasignaturas(ByVal email As String) As SqlDataReader
        Dim st = "select asignaturas.codigo From asignaturas inner join GruposClase on asignaturas.codigo=GruposClase.codigoasig inner join EstudiantesGrupo  on EstudiantesGrupo.Grupo= GruposClase.codigo  where EstudiantesGrupo.Email='" & email & "'"
        comando = New SqlCommand(st, conexion)
        Return (comando.ExecuteReader())
    End Function

    'Devuelve las tareas de una asignatura'
    Public Shared Function tareasXasignatura(ByVal asignatura As String, ByVal email As String) As SqlDataAdapter
        'Dim strSql As String = "Select Codigo, Descripcion, HEstimadas, TipoTarea From TareasGenericas Where CodAsig = " & asignatura
        'strSql += "and Codigo not in (Select Codigo From TareasPersonales Where email=" & email & ")"
        Dim adp As New SqlDataAdapter("Select Codigo, Descripcion, HEstimadas, Explotacion, TipoTarea From TareasGenericas Where CodAsig = '" & asignatura & "'", conexion)
        Return adp
    End Function

    'Devuelve las tareas del alumno
    Public Shared Function alumnoXtarea(ByVal email As String) As SqlDataAdapter

        Dim adp As New SqlDataAdapter("Select Email, CodTarea, HEstimadas, Hreales From EstudiantesTareas Where Email = '" & email & "'", conexion)
        Return adp
    End Function

    'Comprueba que el email no exista en la BD, devuelve true si existe'
    Public Shared Function compruebaEmail(ByVal em As String) As String
        Dim st = "Select confirmado From Usuarios Where email='" & em & "'"
        comando = New SqlCommand(st, conexion)
        Dim resultado As String = comando.ExecuteScalar()
        If resultado = "False" Then
            Return "True"
        ElseIf resultado = "True" Then
            Return "True"
        End If
        Return "False"

    End Function

    'Devuelve las tareas de una asignatura'
    Public Shared Function daTareas(ByVal asignatura As String) As SqlDataAdapter
        Dim adp As New SqlDataAdapter("Select * From TareasGenericas Where Codigo='" & asignatura & "'", conexion)
        adp.InsertCommand = New SqlCommand("Insert into TareasGenericas (Codigo, Descripcion, CodAsig, HEstimadas, Explotacion, TipoTarea) VALUES (@Codigo, @Descripcion, @CodAsig, @HEstimadas, @Explotacion, @TipoTarea )", conexion)
        adp.InsertCommand.Parameters.Add("@Codigo", SqlDbType.NVarChar, 50, "Codigo")
        adp.InsertCommand.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 50, "Descripcion")
        adp.InsertCommand.Parameters.Add("@CodAsig", SqlDbType.NVarChar, 50, "CodAsig")
        adp.InsertCommand.Parameters.Add("@HEstimadas", SqlDbType.Int, 10, "HEstimadas")
        adp.InsertCommand.Parameters.Add("@Explotacion", SqlDbType.Bit, 1, "Explotacion")
        adp.InsertCommand.Parameters.Add("@TipoTarea", SqlDbType.NVarChar, 50, "TipoTarea")


        Return adp
    End Function


    'Validar que el código insertado es correcto, entrada --> email y código'
    Public Shared Function confirm(ByVal email As String, ByVal codigo As String) As String
        Dim st = "Select numconfir from usuarios Where email='" & email & "'"
        comando = New SqlCommand(st, conexion)
        Dim aux = CInt(codigo)
        Dim num = comando.ExecuteScalar
        If num.Equals(aux) Then
            'llamada a confirmado'
            Dim comprobacion = confirmado(email)
            If comprobacion = "1" Then
                Return "Su cuenta ha sido confirmada"
            Else
                Return comprobacion
            End If
        Else
            Return "Su código de confirmación no es correcto"
        End If
    End Function

    'Mira si el usuario está registrado y si en caso afirmativo está confirmado o no'
    Public Shared Function Existe(ByVal email1 As String, ByVal pass1 As String) As String
        Dim st = "Select confirmado From Usuarios Where email='" & email1 & "' And pass='" & pass1 & "' "


        comando = New SqlCommand(st, conexion)
        Dim respuesta As String
        Dim resultado As String = comando.ExecuteScalar()
        If (resultado = "False") Then
            respuesta = "Estas logueado pero no confirmado"
        ElseIf (resultado = "True") Then
            respuesta = "Bienvenido"

        Else
            respuesta = "Datos erroneos"
        End If
        Return respuesta

    End Function

    'Mira que el código insertado por el usuario sea el mismo que el de la base de datos'
    Public Shared Function confirmPass(ByVal email As String, ByVal codigo As String, ByVal pass As String) As String
        Dim st = "Select codpass from usuarios Where email='" & email & "'"
        comando = New SqlCommand(st, conexion)
        Dim aux = CInt(codigo)
        Dim num = comando.ExecuteScalar
        If num.Equals(aux) Then
            'llamada a confirmado'
            Dim comprobacion = confirmadoPass(email, pass)
            If comprobacion = "1" Then

                Return "Su contraseña ha sido cambiada, por favor haz login y comprueba tu nueva contraseña"
            Else
                Return comprobacion
            End If
        Else
            Return "Su código de confirmación no es correcto"
        End If
    End Function

    'devuelve de que tipo es el email'
    Public Shared Function dicetipo(ByVal email As String) As String
        Dim st = "select tipo from Usuarios Where email='" & email & "'"
        comando = New SqlCommand(st, conexion)
        Return comando.ExecuteScalar
    End Function

    'Devuelve el nombre ascoaido al email de entrada'
    Public Shared Function danombre(ByVal email As String) As String
        Dim st = "select nombre from usuarios Where email='" & email & "'"
        comando = New SqlCommand(st, conexion)
        Return comando.ExecuteScalar
    End Function
    Public Shared Function asigXProfe(ByVal email As String) As String
        Dim st = "SELECT Asignaturas.codigo FROM ProfesoresGrupo inner join GruposClase on ProfesoresGrupo.codigogrupo=GruposClase.codigo inner join Asignaturas on GruposClase.codigoasig=Asignaturas.codigo WHERE email='" & email & "'".First
        comando = New SqlCommand(st, conexion)
        Return comando.ExecuteScalar

    End Function

    'MODIFICACIONES (hasta la 191)'

    'Cambia el parametro confirmado en la BD de 0 a 1, entrada --> email'
    Public Shared Function confirmado(ByVal email As String) As String
        Dim st = "update usuarios set confirmado=1 where email='" & email & "'"
        comando = New SqlCommand(st, conexion)
        Dim numregs As Integer
        Try
            numregs = comando.ExecuteNonQuery()
        Catch ex As Exception
            Return ex.Message
        End Try
        'Return (numregs & " reg istro(s) modificado(s) en la BD ")'
        Return "1"
    End Function



    'Añade el código de la contraseña al usuario indicado'
    Public Shared Sub insertarCodPass(ByVal email As String, ByVal codpass As String)

        Dim st = "update usuarios set codpass='" & codpass & "' where email='" & email & "'"
        Dim numregs As Integer
        comando = New SqlCommand(st, conexion)
        Try
            numregs = comando.ExecuteNonQuery()
        Catch ex As Exception

        End Try
    End Sub

    'Cambia la contraseña anterior con la actual (la introducida)'
    Public Shared Function confirmadoPass(ByVal email As String, ByVal pass As String) As String
        Dim st = "update usuarios set pass='" & pass & "' where email='" & email & "'"
        comando = New SqlCommand(st, conexion)
        Dim numregs As Integer
        Try
            numregs = comando.ExecuteNonQuery()
        Catch ex As Exception
            Return ex.Message
        End Try
        'Return (numregs & " registro(s) modificado(s) en la BD ")'
        Return "1"
    End Function




    'ELIMINACIONES'

    'Elimina un usario dado su email'
    Public Shared Function deleteUser(ByVal email As String) As String
        Dim st = "delete from Usuarios Where email='" & email & "'"
        Dim numregs As Integer
        comando = New SqlCommand(st, conexion)
        Try
            numregs = comando.ExecuteNonQuery()
        Catch ex As Exception
            Return ex.Message
        End Try
        If numregs = 1 Then
            Return "Usuario eliminado"
        Else
            Return "Ha ocurrido algo inesperado"
        End If
    End Function

End Class
