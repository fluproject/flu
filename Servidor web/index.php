<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<style type="text/css">
<!--
@import url("css/style.css");
-->
body {
  background-image: url("images/fondo.png");
  background-color: #FFFFFF;
  background-repeat: repeat-x;
}
</style>
</head>
<body>
<br/><div align="center"><a href="http://www.flu-project.com"><img src="images/banner-servidor-web-flu.png" border="none"/></a></div>
<div align="center">
	<table width="800px" height="200px" border="0px" id="gradient-style" style="text-align:center;">
	<tbody>
		<tr>
			<td align="center">
				Welcome to the Flu Control Panel  
				<form id="form1" name="form1" method="post" action="login.php">
					<br>
					<table>
					<tr>
					<td>
					User: 
					</td>
					<td>
					<input type="text" name="user" id="user" style="width:200px; height: 20px">
					</td>
					</tr>
					<tr>
					<td>
					Password: 
					</td>
					<td>
					<input type="password" name="pass" id="pass" style="width:200px; height: 20px">
					</td>
					</tr>
					<tr>
					<td>
					Captcha: 
					</td>
					<td>
					<input type="captcha" name="captcha" id="captcha" style="width:200px; height: 20px"> 
					</td>
					</tr>
					</table>
					<br/>
					<img src="captcha.php"/><br><br>
					<input type="submit" name="aceptar" value="Log in">
					<br><br>
				</form> 
			</td>
		</tr>
	</tbody>
	</table>
</div>
</body>
</html>