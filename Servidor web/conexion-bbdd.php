<?php
$dbhost="localhost";
$dbusuario="root";
$dbpassword="";
$db="flubbdd";
$conexion = mysql_connect($dbhost, $dbusuario, $dbpassword);
mysql_select_db($db, $conexion);
?>