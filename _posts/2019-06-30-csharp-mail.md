---
layout: post
title: C#으로 메일 전송하기
tags:
  - csharp
---

딴 말이 필요 없다. 매우 쉽다.

```csharp
var mail = new System.Net.Mail.MailMessage();
mail.From = new System.Net.Mail.MailAddress("보낸 사람 메일 주소");
mail.To.Add(to);

mail.Subject = "제목";
mail.SubjectEncoding = System.Text.Encoding.UTF8;
mail.Body = "내용";
mail.BodyEncoding = System.Text.Encoding.UTF8;

// gmail의 서버는 "smtp.gmail.com", 네이버의 서버는 "smtp.naver.com"
System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("SMTP 서버", 587);

// 이걸 하지 않으면 서버가 메일을 보내주지 않는다
smtp.EnableSsl = true;

// 시스템에 설정된 인증 정보를 사용하지 않는다
smtp.UseDefaultCredentials = false;

// 이걸 하지 않으면 naver 인증을 받지 못한다
smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

smtp.Credentials = new System.Net.NetworkCredential("보낸 사람 메일 주소", "보낸 사람 메일 패스워드");

smtp.Send(mail);
```

끗.