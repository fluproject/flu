<?php
//Comprobamos que existe una sesión y tiene la variable conectado, si no, redirige a la página principal
session_start();
if(empty($_SESSION['conectado']))
{
   header ("Location: index.php");
   exit;
}
error_reporting(0);

//Se realiza la conexión a la BBDD
//Datos de conexión a la BBBDD
include "conexion-bbdd.php";
?>

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
  background-color: #000000;
  background-repeat: repeat-x;
}
</style>
</head>
<body>
<br/><div align="center"></a><a href="http://www.flu-project.com"><img src="images/banner-servidor-web-flu.png" border="none"/></a></div><br/>
<div align="center" width="100%">
<a href="menuPrincipal.php"><img src="images/boton-menu-principal.png" border="none"/></a>
<a href="configurar-ataque.php"><img src="images/boton-configurar-ataque.png" border="none"/></a>
<a href="configurar-claves.php"><img src="images/claves1.png" border="none"/></a>
<a href="gestionar-usuarios.php"><img src="images/user2.png" border="none"/></a>
<a href="logout.php"><img src="images/logout.png" border="none"/></a>
<?php

$result_a = mysql_query("SELECT user FROM t_usuarios");

if ($row_a = mysql_fetch_array($result_a)){
	echo '<table id="gradient-style" summary="Meeting Results" style="text-align:center;">';
	echo '<thead><tr><th scope="col">User</th><th scope="col">Delete</th></tr><thead><tbody>';
	do {
		echo '<tr><td>'. ($row_a["user"]) .'</td>';
		echo '<td><a href="eliminar-user.php?u='.($row_a["user"]).'" ><img src="images/delete.png" border="0"/></a></td></tr>';	
	} while ($row_a = mysql_fetch_array($result_a));
	echo '</tbody></table>';
}
include "cerrar-conexion-bbdd.php";
?>
<div align="center">
	<table width="800px" height="200px" border="0px" id="gradient-style" style="text-align:center;">
	<tbody>
		<tr>
			<td align="center">
				Introduce el nombre y clave del nuevo usuario  
				<form id="form1" name="form1" method="post" action="add-user.php">
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
					Pass: 
					</td>
					<td>
					<input type="password" name="pass" id="pass" style="width:200px; height: 20px">
					</td>
					</tr>
					</table>
					<br/>
					<input type="submit" name="aceptar" value="Añadir usuario">
					<br><br>
				</form> 
			</td>
		</tr>
	</tbody>
	</table>
</div>
</body>
</html>