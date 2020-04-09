Imports System.Net.Mail

Public Class Class1
    Public Function enviarEmail(ByVal email As String, ByVal numero As String, ByVal entero As Integer) As Boolean
        Try
            Dim from_address As New MailAddress("galanprietomireia@gmail.com", "Alex&Mireia")
            Dim to_address As New MailAddress(email)
            Dim from_pass As String = "mireiagalan3"
            Dim smtp As New SmtpClient
            smtp.Host = "smtp.gmail.com"
            smtp.Port = 587
            smtp.EnableSsl = True
            smtp.UseDefaultCredentials = False
            smtp.Credentials = New System.Net.NetworkCredential(from_address.Address, from_pass)
            Dim message As New MailMessage(from_address, to_address)
            message.Subject = "subject"

            If (entero = 1) Then
                message.Body = "<html><head></head> <body>" + "Hola, gracias por registrarte en nuestra página <br></br> Tu código de confirmación es: " + numero + "<br></br>Tu correo electrónico es: " + email + "</body></html>"
            Else
                message.Body = "<html><head></head> <body>" + "Hola, gracias por confiar en nosotros <br></br> Tu código de confirmación es: " + numero + "<br></br>Tu correo electrónico es: " + email + "</body></html> "
            End If
            message.IsBodyHtml = True
            smtp.Send(message)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
End Class

